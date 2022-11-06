using PersonalWebsiteBuilder;

var localSite = new DirectoryInfo(@"C:\Temp\SitePlace");
var staticResources = new DirectoryInfo(@"C:\Temp\SiteResources");

var site = new WebSite(localSite, staticResources, "https://ahesselbom.se/");

// Create pages

var home = site.CreateRootPage("index.html", "Anders Hesselbom", "Startsidan", "start");
var about = home.CreateChildPage("om", "index.html", "Om Anders Hesselbom", "Om", "om");
var texts = site.CreateRootPage("index.html", "Texter", "Texter", "texter");

// Add common elements

// Center: Meny
// Center: Innehåll

// Right: Meny
// Right: En kopp kaffe
// Right: Om
// Right: Slump
// Right: Följ mig
// Right: Public service


//foreach (var page in site.GetAllPages())
//{
//    Console.WriteLine(page.GetLocalPath());
//    Console.WriteLine(page.GetRemotePath());
//    Console.WriteLine(page.GetDepth());
//    Console.WriteLine(page.GetRemoteStepupToRoot());
//}

// Render

site.Render(); 