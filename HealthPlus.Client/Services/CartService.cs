using HealthPlus.Client.Models.Cart;
using HealthPlus.Client.Models.Common;

namespace HealthPlus.Client.Services;

public class CartService
{
    private readonly ApiService _apiService;

    private CartDto? _cart;

    public event Action? CartChanged;

    public CartService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public CartDto? CurrentCart => _cart;

    public int TotalQuantity =>
        _cart?.TotalQuantity ?? 0;


    public async Task<CartDto?> LoadAsync()
    {
        var response =
            await _apiService.GetAsync<ApiResponse<CartDto>>(
                "api/cart");

        if (response == null)
            throw new Exception("API returned no response.");

        if (!response.Success)
            throw new Exception(response.Message);

        _cart = response.Data;

        CartChanged?.Invoke();

        return _cart;
    }


    public async Task AddAsync(
        int productId,
        int quantity = 1)
    {
        var request = new
        {
            ProductId = productId,
            Quantity = quantity
        };

        var response =
            await _apiService.PostAsync<
                object,
                ApiResponse<CartDto>>(
                "api/cart",
                request);

        if (response == null)
            throw new Exception("API returned no response.");

        if (!response.Success)
            throw new Exception(response.Message);

        _cart = response.Data;

        CartChanged?.Invoke();
    }


    public async Task UpdateAsync(
        int productId,
        int quantity)
    {
        var request = new
        {
            Quantity = quantity
        };

        var response =
            await _apiService.PutAsync<
                object,
                ApiResponse<CartDto>>(
                $"api/cart/{productId}",
                request);

        if (response == null)
            throw new Exception("API returned no response.");

        if (!response.Success)
            throw new Exception(response.Message);

        _cart = response.Data;

        CartChanged?.Invoke();
    }


    public async Task RemoveAsync(int productId)
    {
        await _apiService.DeleteAsync(
            $"api/cart/{productId}");

        await LoadAsync();
    }


    public void Clear()
    {
        _cart = null;

        CartChanged?.Invoke();
    }
}