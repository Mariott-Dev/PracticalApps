using Packt.Shared; // Category, Product

namespace Northwind.Mvc.Models;
public record QueryIndexViewModel
(
    int VisitorCount,
    IList<Category> Categories,
    IList<Product> Products
);
