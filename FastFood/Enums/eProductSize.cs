using System.ComponentModel.DataAnnotations;

namespace FastFood.Enums;

public enum eProductSize
{
    Single = 1,
    Double = 2,
    Triple = 3,
    [Display(Name = "Double Double")]
    DoubleDouble = 4
}
