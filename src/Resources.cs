namespace EvolutionSim;

//ultimate resource manager
//right now, resources are never unloaded.

public static class Resources
{
    private static Dictionary<string, Image> _images = [];
    private static Dictionary<string, Texture2D> _textures = [];
    private static Font _font = 
        LoadFontEx($"{ResourcesFolder}fonts/{FontName}", FontHeight, null, 0);

    private static string GetImagePath(string path)
    {
        return ResourcesFolder + "images/" + path;
    }

    public static Font GetFont()
    {
        return _font;
    }

    public static Image GetImage(string path)
    {
        Image image;

        if (!_images.TryGetValue(path, out image))
        {
            Console.WriteLine($"Image \"{path}\" doesn't exist. Creating...");
            image = LoadImage(GetImagePath(path));
            _images[path] = image;
        }

        return image;
    }
    public static Texture2D GetTexture(string path)
    {
        Texture2D texture;

        if (!_textures.TryGetValue(path, out texture))
        {
            Console.WriteLine($"Texture \"{path}\" doesn't exist. Creating...");

            if (_images.TryGetValue(path, out Image temp))
            {
                Console.WriteLine("Using existing image.");
                texture = LoadTextureFromImage(temp);
            }
            else
            {
                Console.WriteLine("Creating temp image...");
                temp = LoadImage(GetImagePath(path));
                texture = LoadTextureFromImage(temp);
                UnloadImage(temp);
            }

            _textures[path] = texture;
        }

        return texture;
    }

    public static void UnloadAllImages()
    {
        foreach (var image in _images.Values)
            UnloadImage(image); 
        _images.Clear(); 
    }

    public static void UnloadAllTextures()
    {
        foreach (var texture in _textures.Values)
            UnloadTexture(texture); 
        _textures.Clear(); 
    }

}