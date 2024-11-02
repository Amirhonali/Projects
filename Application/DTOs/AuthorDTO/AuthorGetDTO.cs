using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuthorDTO
{
    public class AuthorGetDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateOnly BirthDate { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        public int[] BooksId { get; set; }
    }
}
