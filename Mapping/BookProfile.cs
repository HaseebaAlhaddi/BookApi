using AutoMapper;
using BookApi.Models;
using BookApi.DTOs;
namespace BookApi.Mapping;
public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookResponseDto>()
            .ForMember(
    dest => dest.CategoryName,
    opt => opt.MapFrom(
        src => src.Category != null
            ? src.Category.Name
            : string.Empty));
        CreateMap<CreateBookDto, Book>();
        CreateMap<UpdateBookDto, Book>();
    }
}