﻿using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.Entites;
using WebApplication1.Models.Response;

namespace WebApplication1.DataAccess.Repositories.ArticleRepository
{
    public class ArticleRepository : Repository<Articles>, IArticleRepository
    {
        public ArticleRepository(ProjectDbContext context) : base(context)
        {
        }
        public ProjectDbContext projectDbContext
        {
            get { return _DbContext as ProjectDbContext; }
        }
        public IEnumerable<Articles> GetArticles(string? title, string? searchQuery , int pageNumber , int pageSize)
        {
             IEnumerable<Articles> articles;
            
            if(string.IsNullOrEmpty(title) && string.IsNullOrEmpty(searchQuery))
            {
                articles= GetAll().OrderBy(a => a.Title).Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                return articles;
            }
           if (!string.IsNullOrEmpty(title)&& string.IsNullOrEmpty(searchQuery))
            {
                articles= Find(a => a.Title.Contains(title)).OrderBy(a => a.Title).Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                return articles;
            }
           if(string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(searchQuery))
            {
                articles= Find(a => a.Content.Contains(searchQuery));
                return articles;
            }
                articles = Find(a => a.Content.Contains(searchQuery) && a.Title.Contains(title))
                        .OrderBy(a => a.Title).Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                return articles;
        }
    }
}
