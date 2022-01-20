public static class R
{
	#region Modifications

	public static string Bold(object obj)
	{
		return $"<b>{obj}</b>";
	}

	public static string Italic(object obj)
	{
		return $"<i>{obj}</i>";
	}

	public static string Size(object obj, int size)
	{
		return $"<size={size}>{obj}</size>";
	}

	#endregion

	#region Colors

	public static string Aliceblue(object obj)
	{
		return Make(obj, "#F0F8FFFF");
	}

	public static string Antiquewhite(object obj)
	{
		return Make(obj, "#FAEBD7");
	}

	public static string Aqua(object obj)
	{
		return Make(obj, "#00FFFF");
	}

	public static string Aquamarine(object obj)
	{
		return Make(obj, "#7FFFD4");
	}

	public static string Azure(object obj)
	{
		return Make(obj, "#F0FFFF");
	}

	public static string Beige(object obj)
	{
		return Make(obj, "#F5F5DC");
	}

	public static string Bisque(object obj)
	{
		return Make(obj, "#FFE4C4");
	}

	public static string Black(object obj)
	{
		return Make(obj, "#000000");
	}

	public static string Blanchedalmond(object obj)
	{
		return Make(obj, "#FFEBCD");
	}

	public static string Blue(object obj)
	{
		return Make(obj, "#0000FF");
	}

	public static string Blueviolet(object obj)
	{
		return Make(obj, "#8A2BE2");
	}

	public static string Brown(object obj)
	{
		return Make(obj, "#A52A2A");
	}

	public static string Burlywood(object obj)
	{
		return Make(obj, "#DEB887");
	}

	public static string Cadetblue(object obj)
	{
		return Make(obj, "#5F9EA0");
	}

	public static string Chartreuse(object obj)
	{
		return Make(obj, "#7FFF00");
	}

	public static string Chocolate(object obj)
	{
		return Make(obj, "#D2691E");
	}

	public static string Coral(object obj)
	{
		return Make(obj, "#FF7F50");
	}

	public static string Cornflowerblue(object obj)
	{
		return Make(obj, "#6495ED");
	}

	public static string Cornsilk(object obj)
	{
		return Make(obj, "#FFF8DC");
	}

	public static string Crimson(object obj)
	{
		return Make(obj, "#DC143C");
	}

	public static string Cyan(object obj)
	{
		return Make(obj, "#00FFFF");
	}

	public static string Darkblue(object obj)
	{
		return Make(obj, "#00008B");
	}

	public static string Darkcyan(object obj)
	{
		return Make(obj, "#008B8B");
	}

	public static string Darkgoldenrod(object obj)
	{
		return Make(obj, "#B8860B");
	}

	public static string Darkgray(object obj)
	{
		return Make(obj, "#A9A9A9");
	}

	public static string Darkgreen(object obj)
	{
		return Make(obj, "#006400");
	}

	public static string Darkgrey(object obj)
	{
		return Make(obj, "#A9A9A9");
	}

	public static string Darkkhaki(object obj)
	{
		return Make(obj, "#BDB76B");
	}

	public static string Darkmagenta(object obj)
	{
		return Make(obj, "#8B008B");
	}

	public static string Darkolivegreen(object obj)
	{
		return Make(obj, "#556B2F");
	}

	public static string Darkorange(object obj)
	{
		return Make(obj, "#FF8C00");
	}

	public static string Darkorchid(object obj)
	{
		return Make(obj, "#9932CC");
	}

	public static string Darkred(object obj)
	{
		return Make(obj, "#8B0000");
	}

	public static string Darksalmon(object obj)
	{
		return Make(obj, "#E9967A");
	}

	public static string Darkseagreen(object obj)
	{
		return Make(obj, "#8FBC8F");
	}

	public static string Darkslateblue(object obj)
	{
		return Make(obj, "#483D8B");
	}

	public static string Darkslategray(object obj)
	{
		return Make(obj, "#2F4F4F");
	}

	public static string Darkslategrey(object obj)
	{
		return Make(obj, "#2F4F4F");
	}

	public static string Darkturquoise(object obj)
	{
		return Make(obj, "#00CED1");
	}

	public static string Darkviolet(object obj)
	{
		return Make(obj, "#9400D3");
	}

	public static string Deeppink(object obj)
	{
		return Make(obj, "#FF1493");
	}

	public static string Deepskyblue(object obj)
	{
		return Make(obj, "#00BFFF");
	}

	public static string Dimgray(object obj)
	{
		return Make(obj, "#696969");
	}

	public static string Dimgrey(object obj)
	{
		return Make(obj, "#696969");
	}

	public static string Dodgerblue(object obj)
	{
		return Make(obj, "#1E90FF");
	}

	public static string Firebrick(object obj)
	{
		return Make(obj, "#B22222");
	}

	public static string Floralwhite(object obj)
	{
		return Make(obj, "#FFFAF0");
	}

	public static string Forestgreen(object obj)
	{
		return Make(obj, "#228B22");
	}

	public static string Fuchsia(object obj)
	{
		return Make(obj, "#FF00FF");
	}

	public static string Gainsboro(object obj)
	{
		return Make(obj, "#DCDCDC");
	}

	public static string Ghostwhite(object obj)
	{
		return Make(obj, "#F8F8FF");
	}

	public static string Goldenrod(object obj)
	{
		return Make(obj, "#DAA520");
	}

	public static string Gold(object obj)
	{
		return Make(obj, "#FFD700");
	}

	public static string Gray(object obj)
	{
		return Make(obj, "#808080");
	}

	public static string Green(object obj)
	{
		return Make(obj, "#008000");
	}

	public static string Greenyellow(object obj)
	{
		return Make(obj, "#ADFF2F");
	}

	public static string Grey(object obj)
	{
		return Make(obj, "#808080");
	}

	public static string Honeydew(object obj)
	{
		return Make(obj, "#F0FFF0");
	}

	public static string Hotpink(object obj)
	{
		return Make(obj, "#FF69B4");
	}

	public static string Indianred(object obj)
	{
		return Make(obj, "#CD5C5C");
	}

	public static string Indigo(object obj)
	{
		return Make(obj, "#4B0082");
	}

	public static string Ivory(object obj)
	{
		return Make(obj, "#FFFFF0");
	}

	public static string Khaki(object obj)
	{
		return Make(obj, "#F0E68C");
	}

	public static string Lavenderblush(object obj)
	{
		return Make(obj, "#FFF0F5");
	}

	public static string Lavender(object obj)
	{
		return Make(obj, "#E6E6FA");
	}

	public static string Lawngreen(object obj)
	{
		return Make(obj, "#7CFC00");
	}

	public static string Lemonchiffon(object obj)
	{
		return Make(obj, "#FFFACD");
	}

	public static string Lightblue(object obj)
	{
		return Make(obj, "#ADD8E6");
	}

	public static string Lightcoral(object obj)
	{
		return Make(obj, "#F08080");
	}

	public static string Lightcyan(object obj)
	{
		return Make(obj, "#E0FFFF");
	}

	public static string Lightgoldenrodyellow(object obj)
	{
		return Make(obj, "#FAFAD2");
	}

	public static string Lightgray(object obj)
	{
		return Make(obj, "#D3D3D3");
	}

	public static string Lightgreen(object obj)
	{
		return Make(obj, "#90EE90");
	}

	public static string Lightgrey(object obj)
	{
		return Make(obj, "#D3D3D3");
	}

	public static string Lightpink(object obj)
	{
		return Make(obj, "#FFB6C1");
	}

	public static string Lightsalmon(object obj)
	{
		return Make(obj, "#FFA07A");
	}

	public static string Lightseagreen(object obj)
	{
		return Make(obj, "#20B2AA");
	}

	public static string Lightskyblue(object obj)
	{
		return Make(obj, "#87CEFA");
	}

	public static string Lightslategray(object obj)
	{
		return Make(obj, "#778899");
	}

	public static string Lightslategrey(object obj)
	{
		return Make(obj, "#778899");
	}

	public static string Lightsteelblue(object obj)
	{
		return Make(obj, "#B0C4DE");
	}

	public static string Lightyellow(object obj)
	{
		return Make(obj, "#FFFFE0");
	}

	public static string Lime(object obj)
	{
		return Make(obj, "#00FF00");
	}

	public static string Limegreen(object obj)
	{
		return Make(obj, "#32CD32");
	}

	public static string Linen(object obj)
	{
		return Make(obj, "#FAF0E6");
	}

	public static string Magenta(object obj)
	{
		return Make(obj, "#FF00FF");
	}

	public static string Maroon(object obj)
	{
		return Make(obj, "#800000");
	}

	public static string Mediumaquamarine(object obj)
	{
		return Make(obj, "#66CDAA");
	}

	public static string Mediumblue(object obj)
	{
		return Make(obj, "#0000CD");
	}

	public static string Mediumorchid(object obj)
	{
		return Make(obj, "#BA55D3");
	}

	public static string Mediumpurple(object obj)
	{
		return Make(obj, "#9370DB");
	}

	public static string Mediumseagreen(object obj)
	{
		return Make(obj, "#3CB371");
	}

	public static string Mediumslateblue(object obj)
	{
		return Make(obj, "#7B68EE");
	}

	public static string Mediumspringgreen(object obj)
	{
		return Make(obj, "#00FA9A");
	}

	public static string Mediumturquoise(object obj)
	{
		return Make(obj, "#48D1CC");
	}

	public static string Mediumvioletred(object obj)
	{
		return Make(obj, "#C71585");
	}

	public static string Midnightblue(object obj)
	{
		return Make(obj, "#191970");
	}

	public static string Mintcream(object obj)
	{
		return Make(obj, "#F5FFFA");
	}

	public static string Mistyrose(object obj)
	{
		return Make(obj, "#FFE4E1");
	}

	public static string Moccasin(object obj)
	{
		return Make(obj, "#FFE4B5");
	}

	public static string Navajowhite(object obj)
	{
		return Make(obj, "#FFDEAD");
	}

	public static string Navy(object obj)
	{
		return Make(obj, "#000080");
	}

	public static string Oldlace(object obj)
	{
		return Make(obj, "#FDF5E6");
	}

	public static string Olive(object obj)
	{
		return Make(obj, "#808000");
	}

	public static string Olivedrab(object obj)
	{
		return Make(obj, "#6B8E23");
	}

	public static string Orange(object obj)
	{
		return Make(obj, "#FFA500");
	}

	public static string Orangered(object obj)
	{
		return Make(obj, "#FF4500");
	}

	public static string Orchid(object obj)
	{
		return Make(obj, "#DA70D6");
	}

	public static string Palegoldenrod(object obj)
	{
		return Make(obj, "#EEE8AA");
	}

	public static string Palegreen(object obj)
	{
		return Make(obj, "#98FB98");
	}

	public static string Paleturquoise(object obj)
	{
		return Make(obj, "#AFEEEE");
	}

	public static string Palevioletred(object obj)
	{
		return Make(obj, "#DB7093");
	}

	public static string Papayawhip(object obj)
	{
		return Make(obj, "#FFEFD5");
	}

	public static string Peachpuff(object obj)
	{
		return Make(obj, "#FFDAB9");
	}

	public static string Peru(object obj)
	{
		return Make(obj, "#CD853F");
	}

	public static string Pink(object obj)
	{
		return Make(obj, "#FFC0CB");
	}

	public static string Plum(object obj)
	{
		return Make(obj, "#DDA0DD");
	}

	public static string Powderblue(object obj)
	{
		return Make(obj, "#B0E0E6");
	}

	public static string Purple(object obj)
	{
		return Make(obj, "#800080");
	}

	public static string Rebeccapurple(object obj)
	{
		return Make(obj, "#663399");
	}

	public static string Red(object obj)
	{
		return Make(obj, "#FF0000");
	}

	public static string Rosybrown(object obj)
	{
		return Make(obj, "#BC8F8F");
	}

	public static string Royalblue(object obj)
	{
		return Make(obj, "#4169E1");
	}

	public static string Saddlebrown(object obj)
	{
		return Make(obj, "#8B4513");
	}

	public static string Salmon(object obj)
	{
		return Make(obj, "#FA8072");
	}

	public static string Sandybrown(object obj)
	{
		return Make(obj, "#F4A460");
	}

	public static string Seagreen(object obj)
	{
		return Make(obj, "#2E8B57");
	}

	public static string Seashell(object obj)
	{
		return Make(obj, "#FFF5EE");
	}

	public static string Sienna(object obj)
	{
		return Make(obj, "#A0522D");
	}

	public static string Silver(object obj)
	{
		return Make(obj, "#C0C0C0");
	}

	public static string Skyblue(object obj)
	{
		return Make(obj, "#87CEEB");
	}

	public static string Slateblue(object obj)
	{
		return Make(obj, "#6A5ACD");
	}

	public static string Slategray(object obj)
	{
		return Make(obj, "#708090");
	}

	public static string Slategrey(object obj)
	{
		return Make(obj, "#708090");
	}

	public static string Snow(object obj)
	{
		return Make(obj, "#FFFAFA");
	}

	public static string Springgreen(object obj)
	{
		return Make(obj, "#00FF7F");
	}

	public static string Steelblue(object obj)
	{
		return Make(obj, "#4682B4");
	}

	public static string Tan(object obj)
	{
		return Make(obj, "#D2B48C");
	}

	public static string Teal(object obj)
	{
		return Make(obj, "#008080");
	}

	public static string Thistle(object obj)
	{
		return Make(obj, "#D8BFD8");
	}

	public static string Tomato(object obj)
	{
		return Make(obj, "#FF6347");
	}

	public static string Turquoise(object obj)
	{
		return Make(obj, "#40E0D0");
	}

	public static string Violet(object obj)
	{
		return Make(obj, "#EE82EE");
	}

	public static string Wheat(object obj)
	{
		return Make(obj, "#F5DEB3");
	}

	public static string White(object obj)
	{
		return Make(obj, "#FFFFFF");
	}

	public static string Whitesmoke(object obj)
	{
		return Make(obj, "#F5F5F5");
	}

	public static string Yellow(object obj)
	{
		return Make(obj, "#FFFF00");
	}

	public static string Yellowgreen(object obj)
	{
		return Make(obj, "#9ACD32");
	}

	public static string RoseGold(object obj)
	{
		return Make(obj, "#D99090");
	}


	public static string Custom(object obj, string hex)
	{
		return Make(obj, $"#{hex}");
	}

	private static string Make(object obj, string color)
	{
		return $"<color={color}>{obj}</color>";
	}

	#endregion
}