using AiQaLab.AI.Services;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace AiQaLab.Tests.Unit.AI
{
    public class OllamaAIClientTests
    {
        [Fact]
        public async Task SendAsync_ReturnsContentFromOllamaResponse()
        {
            var responseJson = """
            {
                "message": {
                    "role": "assistant",
                    "content": "Hello from Ollama"
                }
            }
            """;

            var handler = new Mock<HttpMessageHandler>();

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(request =>
                        request.Method == HttpMethod.Post &&
                        request.RequestUri!.AbsolutePath == "/api/chat"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        responseJson,
                        Encoding.UTF8,
                        "application/json")
                });

            var httpClient = new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("http://localhost:11434")
            };

            var client = new OllamaAIClient(
                httpClient,
                "qwen3:4b");

            var result = await client.SendAsync("Hello");

            result.Should().Be("Hello from Ollama");
        }

        [Fact]
        public async Task SendStructuredAsync_ReturnsContentFromOllamaResponse()
        {
            var responseJson = """
            {
                "message": {
                    "role": "assistant",
                    "content": "{\"suggestions\":[{\"description\":\"Test password boundary\",\"testLevel\":\"Unit\",\"reason\":\"Tests the minimum valid length.\"}]}"
                }
            }
            """;

            var responseSchema = JsonNode.Parse("""
            {
                "type": "object",
                "properties": {
                    "suggestions": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "description": {
                                    "type": "string"
                                },
                                "testLevel": {
                                    "type": "string",
                                    "enum": ["Unit", "Integration", "EndToEnd"]
                                },
                                "reason": {
                                    "type": "string"
                                }
                            },
                            "required": ["description", "testLevel", "reason"]
                        },
                        "minItems": 1
                    }
                },
                "required": ["suggestions"]
            }
            """)!;

            var handler = new Mock<HttpMessageHandler>();

            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(request =>
                        request.Method == HttpMethod.Post &&
                        request.RequestUri!.AbsolutePath == "/api/chat"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        responseJson,
                        Encoding.UTF8,
                        "application/json")
                });

            var httpClient = new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("http://localhost:11434")
            };

            var client = new OllamaAIClient(
                httpClient,
                "qwen3:4b");

            var result = await client.SendStructuredAsync(
                "Analyze this requirement.",
                responseSchema);

            result.Should().Be(
                "{\"suggestions\":[{\"description\":\"Test password boundary\",\"testLevel\":\"Unit\",\"reason\":\"Tests the minimum valid length.\"}]}");
        }

        [Fact]
        public async Task SendStructuredAsync_SendsResponseSchemaAsFormat()
        {
            var responseJson = """
            {
                "message": {
                    "role": "assistant",
                    "content": "{\"result\":\"test\"}"
                }
            }
            """;

            var responseSchema = JsonNode.Parse("""
            {
                "type": "object",
                "properties": {
                    "result": {
                        "type": "string"
                    }
                },
                "required": ["result"]
            }
            """)!;

            var handler = new CapturingHttpMessageHandler
            {
                Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        responseJson,
                        Encoding.UTF8,
                        "application/json")
                }
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:11434")
            };

            var client = new OllamaAIClient(
                httpClient,
                "qwen3:4b");

            var result = await client.SendStructuredAsync("Return a test result.", responseSchema);

            result.Should().NotBeNullOrWhiteSpace();
            handler.Request.Should().NotBeNull();

            var requestBody = await handler.Request!.Content!.ReadFromJsonAsync<JsonNode>();

            requestBody!["model"]!.GetValue<string>().Should().Be("qwen3:4b");

            requestBody["stream"]!.GetValue<bool>().Should().BeFalse();

            requestBody["format"]!.ToJsonString().Should().Be(responseSchema.ToJsonString());
        }

        private class CapturingHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? Request { get; private set; }

            public HttpResponseMessage Response { get; set; } = new();

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Request = request;
                return Task.FromResult(Response);
            }
        }
    }
}
