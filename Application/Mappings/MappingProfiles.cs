using Application.DTOs.AuthorDTO;
using Application.DTOs.BookDTO;
using Application.DTOs.PermissionDTO;
using Application.DTOs.RoleDTO;
using Application.DTOs.UserDTO;
using Application.UseCases.Book.Commands;
using Application.UseCases.Book.Queries;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            BookMappingRules();
            AuthorMappingRules();
            UserMappingRules();
            PermissionMappingRules();
            RoleMappingRules();
        }
        private void UserMappingRules()
        {
            CreateMap<UserCreateDTO, User>()
                .ForMember(dest => dest.Roles,
                           opt => opt.MapFrom(src => src.RolesId
                              .Select(x => new Role() { RoleId = x })));

            CreateMap<User, UserGetDTO>()
                .ForMember(dest => dest.RolesId,
                           opt => opt.MapFrom(src => src.Roles.Select(x => x.RoleId)));

            CreateMap<UserUpdateDTO, User>()
               .ForMember(dest => dest.Roles,
                          opt => opt.MapFrom(src => src.RolesId
                             .Select(x => new Role() { RoleId = x })));
        }
        private void RoleMappingRules()
        {
            CreateMap<RoleCreateDTO, Role>()
                .ForMember(dest => dest.Permissions,
                           opt => opt.MapFrom(src => src.PermissionsId
                              .Select(x => new Permission() { PermissionId = x })));

            CreateMap<Role, RoleGetDTO>()
                .ForMember(dest => dest.PermissionsId,
                           opt => opt.MapFrom(src => src.Permissions.Select(x => x.PermissionId)));

            CreateMap<RoleUpdateDTO, Role>()
                .ForMember(dest => dest.Permissions,
                           opt => opt.MapFrom(src => src.PermissionsId
                              .Select(x => new Permission() { PermissionId = x })));
        }

        private void PermissionMappingRules()
        {
            CreateMap<PermissionCreateDTO, Permission>();
        }



        public void BookMappingRules()
        {
            CreateMap<BookCreateDTO, Book>()
                   .ForMember(dest => dest.Authors,
                              opt => opt.MapFrom(src => src.AuthorsId
                              .Select(x => new Author() { Id = x })))
                   .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.PublishDate)));

            CreateMap<CreateBookCommand , Book>()
                   .ForMember(dest => dest.Authors,
                              opt => opt.MapFrom(src => src.AuthorsId
                              .Select(x => new Author() { Id = x })))
                   .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.PublishDate)));


            CreateMap<BookUpdateDTO, Book>()
                   //.ForMember(dest => dest.Authors,
                   //           opt => opt.MapFrom(src => src.AuthorsId
                   //           .Select(x => new Author() { Id = x })))
                   .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.PublishDate)));

            CreateMap<UpdateBookCommand, Book>()
                   //.ForMember(dest => dest.Authors,
                   //           opt => opt.MapFrom(src => src.AuthorsId
                   //           .Select(x => new Author() { Id = x })))
                   .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.PublishDate)));


            CreateMap<Book, BookGetDTO>()
                .ForMember(dest => dest.AuthorsId,
                opt => opt.MapFrom(src => src.Authors.Select(x => x.Id)));

            CreateMap<Book, GetAllBooksQueryResult>()
               .ForMember(dest => dest.AuthorsId,
               opt => opt.MapFrom(src => src.Authors.Select(x => x.Id)));

            CreateMap<Book, CreateBookCommandResult>()
                .ForMember(dest => dest.AuthorsId,
                opt => opt.MapFrom(src => src.Authors.Select(x => x.Id)));

            CreateMap<Book, UpdateBookCommandResult>()
                .ForMember(dest => dest.AuthorsId,
                opt => opt.MapFrom(src => src.Authors.Select(x => x.Id)));
        }

        public void AuthorMappingRules()
        {
            CreateMap<Author, AuthorGetDTO>()
                .ForMember(dest => dest.BooksId,
                opt => opt.MapFrom(src => src.Books.Select(x => x.Id)));

            CreateMap<AuthorCreateDTO, Author>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.BirthDate)));


            CreateMap<AuthorUpdateDTO, Author>()
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.BirthDate)));


        }
    }


}
