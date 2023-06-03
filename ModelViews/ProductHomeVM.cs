
using Business.Data;

namespace Supermarket.ModelViews
{
    public class ProductHomeVM
    {
        public Category category { get; set; }
        public List<Product> lsProducts { get; set; }
    }
}
