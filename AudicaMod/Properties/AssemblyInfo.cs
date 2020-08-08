using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using HtiScoreVisualizerMod;

[assembly: AssemblyTitle(HtiScoreVisualizer.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(HtiScoreVisualizer.BuildInfo.Company)]
[assembly: AssemblyProduct(HtiScoreVisualizer.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + HtiScoreVisualizer.BuildInfo.Author)]
[assembly: AssemblyTrademark(HtiScoreVisualizer.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(HtiScoreVisualizer.BuildInfo.Version)]
[assembly: AssemblyFileVersion(HtiScoreVisualizer.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(HtiScoreVisualizer), HtiScoreVisualizer.BuildInfo.Name, HtiScoreVisualizer.BuildInfo.Version, HtiScoreVisualizer.BuildInfo.Author, HtiScoreVisualizer.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]