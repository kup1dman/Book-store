using BookStore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStore.Models.ViewModels.Shop
{
    public class CategoryViewModel
    {
        public CategoryViewModel() { }

        public CategoryViewModel(CategoryDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Description = row.Description;
            Sorting = row.Sorting;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Sorting { get; set; }

    }
}