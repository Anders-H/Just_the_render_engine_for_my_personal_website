using System;

namespace AhesselbomGenerator;

public class StaticMenuProcessor
{
    private readonly string _selectedItemName;

    public StaticMenuProcessor(string selectedItemName)
    {
        _selectedItemName = selectedItemName;
    }

    public string Generate()
    {
        switch (_selectedItemName)
        {
            case "Start":
            case "Texter":
            case "YouTube":
            case "Twitter":
            case "Podcast":
            case "HallOfFame":
            case "Evolution":
                return $@"<div id=""mainMenu""><table id=""mainMenuTable""><tr>
<td class=""menuCell"" style=""width: 15%;""><a href=""https://ahesselbom.se/home/"" class=""topMenuLink{(_selectedItemName == "Start" ? " selected" : "")}"">Startsidan</a></td>
<td class=""menuCell"" style=""width: 14%;""><a href=""https://ahesselbom.se/texter/"" class=""topMenuLink{(_selectedItemName == "Texter" ? " selected" : "")}"">Texter</a></td>
<td class=""menuCell"" style=""width: 14%;""><a href=""https://ahesselbom.se/youtube/"" class=""topMenuLink{(_selectedItemName == "YouTube" ? " selected" : "")}"">YouTube</a></td>
<td class=""menuCell"" style=""width: 14%;""><a href=""https://ahesselbom.se/twitter/"" class=""topMenuLink{(_selectedItemName == "Twitter" ? " selected" : "")}"">Twitter</a></td>
<td class=""menuCell"" style=""width: 14%;""><a href=""https://ahesselbom.se/podcast/"" class=""topMenuLink{(_selectedItemName == "Podcast" ? " selected" : "")}"">Podcast</a></td>
<td class=""menuCell"" style=""width: 15%;""><a href=""https://ahesselbom.se/hall-of-fame/"" class=""topMenuLink{(_selectedItemName == "HallOfFame" ? " selected" : "")}"">Hall of fame</a></td>
<td class=""menuCell"" style=""width: 14%;""><a href=""https://ahesselbom.se/evolution/"" class=""topMenuLink{(_selectedItemName == "Evolution" ? " selected" : "")}"">Evolution</a></td>
</tr></table></div>";
            default:
                throw new SystemException();
        }
    }
}