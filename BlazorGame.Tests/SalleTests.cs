using SharedModels;

namespace BlazorGame.Tests
{
    public class SalleTests
    {
        [Fact]
        public void Room()
        {
            var salle = new Room();
            Assert.Equal("Une salle vide", salle.Description);
        }
    }
}