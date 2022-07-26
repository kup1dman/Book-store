﻿using BookStore.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Models.ViewModels.Shop
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {

        }

        public ProductViewModel(ProductDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            ImageName = row.ImageName;
        }

        public int Id { get; set; }

        [Required]
        [DisplayName("Имя")]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required]
        [DisplayName("Описание")]
        [AllowHtml]
        public string Description { get; set; }

        [DisplayName("Цена")]
        public decimal Price { get; set; }

        public string CategoryName { get; set; }

        [Required]
        [DisplayName("Категория")]
        public int CategoryId { get; set; }

        [DisplayName("Изображение")]
        public string ImageName { get; set; }


        public IEnumerable<SelectListItem> Categories { get; set; }

        public IEnumerable<string> GalleryImages { get; set; }

    }
}