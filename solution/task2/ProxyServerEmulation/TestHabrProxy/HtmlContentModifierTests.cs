using HabrProxy.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace TestHabrProxy
{
    public class HtmlContentModifierTests
    {
        [Fact]
        public void Modify_null_object_should_return_empty_string()
        {
            // arrange
            string content = null;
            var mock = new Mock<IWebHostEnvironment>();
            var modifier = new HtmlContentModifier(mock.Object);

            // act
            var actual = modifier.Modify(content);

            // assert
            Assert.Equal(string.Empty, actual);
        }


        [Theory]
        [InlineData(" привет блабла yesokay hi", " привет™ блабла™ yesokay hi")]
        [InlineData("привет блабла yesokay hi", "привет™ блабла™ yesokay hi")]
        [InlineData("Очень2 крутая строка", "Очень2 крутая™ строка™")]
        [InlineData("Привет", "Привет™")]
        [InlineData("Здравствуйте", "Здравствуйте")]
        [InlineData("1337", "1337")]
        [InlineData("Helloo world", "Helloo™ world")]
        public void Modify_test_different_input_cases(string content, string expected) 
        {
            // arrange
            var mock = new Mock<IWebHostEnvironment>();
            var modifier = new HtmlContentModifier(mock.Object);

            // act
            var actual = modifier.Modify(content);

            // assert
            Assert.Equal(expected, actual);
        }
        
    }
}