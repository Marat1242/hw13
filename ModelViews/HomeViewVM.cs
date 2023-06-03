using Business.Data;

namespace Supermarket.ModelViews
{
    public class HomeViewVM
    {
        public List<TinDang> News { get; set; }
        public List<ProductHomeVM> Products { get; set; }
        public Advertise Advertise { get; set; }
    }
}
