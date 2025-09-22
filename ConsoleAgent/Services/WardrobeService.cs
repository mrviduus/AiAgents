
namespace ConsoleAgent.Services;

public class WardrobeService()
{
    public Task<string[]> ListClothing()
    {
        return Task.FromResult<string[]>([
            "Warm Coat",
            "Light Jacket",
            "Metallica T Shirt",
            "Long Scarf",
            "Wolly Hat",
            "Jeans",
            "Bermuda Shorts"
        ]);
    }
}

