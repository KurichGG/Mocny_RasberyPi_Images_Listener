using MQTTnet;
using System.Text.Json;

namespace Mocny_RasberyPi_Images_Listener.Services
{
    public class MqttPublisherService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MqttPublisherService> _logger;
        private IMqttClient? _mqttClient;

        public MqttPublisherService(IConfiguration configuration, ILogger<MqttPublisherService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private async Task<IMqttClient> GetClientAsync()
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
                return _mqttClient;

            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();

            var host = _configuration["Mqtt:Host"];
            var port = int.Parse(_configuration["Mqtt:Port"]!);
            var username = _configuration["Mqtt:Username"];
            var password = _configuration["Mqtt:Password"];

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .WithCredentials(username, password)
                .WithClientId($"backend-{Guid.NewGuid()}")
                .WithCleanSession()
                .Build();

            await _mqttClient.ConnectAsync(options, CancellationToken.None);
            _logger.LogInformation("Połączono z brokerem MQTT");

            return _mqttClient;
        }

        public async Task PublishCommandAsync(string screenId, object command)
        {
            try
            {
                var client = await GetClientAsync();
                var topic = $"screens/{screenId}/command";
                var payload = JsonSerializer.Serialize(command);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithRetainFlag(false)
                    .Build();

                await client.PublishAsync(message, CancellationToken.None);
                _logger.LogInformation($"Wysłano polecenie do {topic}: {payload}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd publikacji MQTT");
            }
        }

        public async Task PublishToAllAsync(object command)
        {
            try
            {
                var client = await GetClientAsync();
                var topic = "screens/all/command";
                var payload = JsonSerializer.Serialize(command);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .Build();

                await client.PublishAsync(message, CancellationToken.None);
                _logger.LogInformation($"Wysłano polecenie do wszystkich: {payload}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd publikacji MQTT (all)");
            }
        }
    }
}