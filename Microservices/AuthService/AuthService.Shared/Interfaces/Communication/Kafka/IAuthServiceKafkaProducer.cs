namespace AuthService.Shared.Interfaces.Communication.Kafka
{
    public interface IAuthServiceKafkaProducer
    {
        Task ForceDeleteAspNetUserAsync(string userId);
    }
}
