using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using CustomJsonConverters;

[TestFixture]
public class MultiObjectDuplicateKeyConverterTests
{
    public class RecipientList
    {
        public List<string> recipient { get; set; } = new List<string>();
    }
    
    [Test]
    public void SingleObjectWithDuplicateKeys_ShouldAggregateValues()
    {
        // Arrange
        string json = @"{ ""recipient"": ""George"", ""recipient"": ""Adam"" }";
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new MultiObjectDuplicateKeyConverter<RecipientList>());

        // Act
        var result = JsonConvert.DeserializeObject<List<RecipientList>>(json, settings);

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(2, result[0].recipient.Count);
        Assert.Contains("George", result[0].recipient);
        Assert.Contains("Adam", result[0].recipient);
    }

    [Test]
    public void MultipleObjectsWithDuplicateKeys_ShouldAggregateValuesPerObject()
    {
        // Arrange
        string json = @"{ ""recipient"": ""George"", ""recipient"": ""Adam"" }
                        { ""recipient"": ""Rob"", ""recipient"": ""Josh"", ""recipient"":""Dean"" }";
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new MultiObjectDuplicateKeyConverter<RecipientList>());

        // Act
        var result = JsonConvert.DeserializeObject<List<RecipientList>>(json, settings);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result[0].recipient.Count);
        Assert.AreEqual(3, result[1].recipient.Count);

        Assert.Contains("George", result[0].recipient);
        Assert.Contains("Adam", result[0].recipient);

        Assert.Contains("Rob", result[1].recipient);
        Assert.Contains("Josh", result[1].recipient);
        Assert.Contains("Dean", result[1].recipient);
    }

    [Test]
    public void EmptyJsonString_ShouldReturnNull()
    {
        // Arrange
        string json = @"";
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new MultiObjectDuplicateKeyConverter<RecipientList>());

        // Act
        var result = JsonConvert.DeserializeObject<List<RecipientList>>(json, settings);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void SingleObjectWithNoDuplicateKeys_ShouldDeserializeCorrectly()
    {
        // Arrange
        string json = @"{ ""recipient"": ""George"" }";
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new MultiObjectDuplicateKeyConverter<RecipientList>());

        // Act
        var result = JsonConvert.DeserializeObject<List<RecipientList>>(json, settings);

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].recipient.Count);
        Assert.Contains("George", result[0].recipient);
    }

    [Test]
    public void MultipleObjectsWithMixedDuplicateAndUniqueKeys_ShouldDeserializeCorrectly()
    {
        // Arrange
        string json = @"{ ""recipient"": ""George"", ""recipient"": ""Adam"" }
                        { ""recipient"": ""Rob"" }";
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new MultiObjectDuplicateKeyConverter<RecipientList>());

        // Act
        var result = JsonConvert.DeserializeObject<List<RecipientList>>(json, settings);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(2, result[0].recipient.Count);
        Assert.AreEqual(1, result[1].recipient.Count);

        Assert.Contains("George", result[0].recipient);
        Assert.Contains("Adam", result[0].recipient);
        Assert.Contains("Rob", result[1].recipient);
    }
}
