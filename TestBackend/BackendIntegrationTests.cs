﻿using System.Net;
using System.Text;
using Domain.DTOs;

using Newtonsoft.Json;

namespace TestBackend;

public class BackendIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    public BackendIntegrationTests(CustomWebApplicationFactory clientFactory)
    {
        clientFactory.ClientOptions.BaseAddress = new Uri("https://localhost:5144");
        _httpClient = clientFactory.CreateClient();
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsOkResult()
    {
        //Arrange
        var expectedStatusCode = HttpStatusCode.OK;
        var expectedContentType = "application/json";

        //Act
        using var response = await _httpClient.GetAsync("/api/users");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.IsType<string>(responseBody);
        Assert.NotEqual(string.Empty, responseBody);
        Assert.Equal(expectedStatusCode, response.StatusCode);
        Assert.Contains(expectedContentType, response.Content.Headers.ContentType.ToString());
        
    }

    
    [Fact]
    public async Task CreateNewUserAsync_ReturnsOkResult()
    {
        //Arrange
        var newUser = new UserCreateDto{ Name = "Test", Email = "test@test.com", Username = "test" };

        //Act
        var jsonData = JsonConvert.SerializeObject(newUser);
        using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync("/api/users", content);
         response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var deserializedResult = JsonConvert.DeserializeObject<UserDto>(result);
        
        //Assert
        Assert.IsType<UserDto>(deserializedResult);
        Assert.NotNull(deserializedResult);
        Assert.Equal(newUser.Name, deserializedResult.Name);
        Assert.Equal(newUser.Email, deserializedResult.Email);
        Assert.Equal(newUser.Username, deserializedResult.Username);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetModuleWithIdAsync_ReturnsOkResult()
    {
        //Arrange
        int id = 1;
        
        //Act
        using var response = await _httpClient.GetAsync($"/api/modules/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}