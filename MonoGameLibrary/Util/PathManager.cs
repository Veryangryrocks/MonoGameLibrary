using System.IO;

namespace MonoGameLibrary.Util;

public static class PathManager
{
    public static readonly string RootDir = "../../../../";
    public static readonly string LibDir = Path.Join(RootDir, "MonoGameLibrary" + "/");
    public static readonly string LibDataDir = Path.Join(LibDir, "Data/");
    public static readonly string ProjectDir = Path.Join(RootDir, "Testing" + "/");
    public static readonly string ProjectDataDir = Path.Join(ProjectDir, "Data/");
}