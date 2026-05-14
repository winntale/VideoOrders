using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideoOrdersPortal.Models;

namespace VideoOrdersPortal.VIews.Home;

public class IndexModel : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    [BindProperty] public CreateOrderDto Input { get; set; } = new();
    public List<OrderDto> Orders { get; set; } = [];

    public IndexModel(IHttpClientFactory factory, IConfiguration config)
    {
        _httpClient = factory.CreateClient("Gateway");
        _config = config;
    }

    public async Task OnGetAsync()
    {
        // Получи orders юзера (добавь endpoint /Orders/List?userId=...)
        Orders = await _httpClient.GetFromJsonAsync<List<OrderDto>>($"{_config["Gateway:BaseUrl"]}/Orders") ?? [];
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        // Вызов Gateway/Orders/CreateAsync
        var response = await _httpClient.PostAsJsonAsync($"{_config["Gateway:BaseUrl"]}/Orders/Create", Input);
        return response.IsSuccessStatusCode ? RedirectToPage() : BadRequest();
    }
}