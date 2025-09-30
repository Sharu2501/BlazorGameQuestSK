using SharedModels;

namespace BlazorGame.Tests
{
    public class SalleTests
    {
        [Fact]
        public void Salle_Par_Defaut()
        {
            var salle = new Salle();
            Assert.Equal("Une salle vide", salle.Description);
        }
    }
}