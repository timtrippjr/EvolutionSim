namespace EvolutionSim;

//ultimate resource manager
//right now, resources are never unloaded.

public static class Resources
{
    private static Dictionary<string, Image> _images = [];
    private static Dictionary<string, Texture2D> _textures = [];
    private static Dictionary<string, Shader> _shaders = [];
    private static Dictionary<string, Sound> _sounds = [];

    private static Font _font = 
        LoadFontEx($"{ResourcesFolder}fonts/{FontName}", FontHeight, null, 0);

    private static string GetPath(string folder, string filename)
    {
        return ResourcesFolder + folder + filename;
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
            image = LoadImage(GetPath(ImagesFolder, path));
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
                temp = LoadImage(GetPath(ImagesFolder, path));
                texture = LoadTextureFromImage(temp);
                UnloadImage(temp);
            }

            _textures[path] = texture;
        }

        return texture;
    }

    public static Shader GetShader(string path)
    {
        Shader shader;

        if (!_shaders.TryGetValue(path, out shader))
        {
            Console.WriteLine($"Shader \"{path}\" doesn't exist. Creating...");
            shader = LoadShader(null, GetPath(ShadersFolder, path));
            _shaders[path] = shader;
        }

        return shader;
    }

    public static Sound GetSound(string path)
    {
        Sound sound;

        if (!_sounds.TryGetValue(path, out sound))
        {
            Console.WriteLine($"Sound \"{path}\" doesn't exist. Loading...");
            sound = LoadSound(GetPath(SoundsFolder, path));
            _sounds[path] = sound;
        }

        return sound;
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