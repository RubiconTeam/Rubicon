using System.Collections;
using System.Collections.Generic;
using Godot.Collections;

namespace Rubicon.Screens;

/// <summary>
/// Used to keep track of what resources to load.
/// </summary>
[GlobalClass] public partial class ResourceLoadList : Resource, IReadOnlyList<string>
{
    [Export(PropertyHint.File)] private Array<string> _paths = [];
    
    public void AddResource(string path) => TryAdd(GetResourcePath(path));

    public void AddScene(string path) => TryAdd(GetScenePath(path));
    
    public void AddAudio(string path) => TryAdd(GetAudioPath(path));
    
    public void AddPath(string path) => TryAdd(path);

    public void RemoveResource(string path) => TryRemove(GetResourcePath(path));
    
    public void RemoveScene(string path) => TryRemove(GetScenePath(path));
    
    public void RemoveAudio(string path) => TryRemove(GetAudioPath(path));

    public void RemovePath(string path) => TryRemove(path);
    
    public bool ContainsResource(string path) => TryContains(GetResourcePath(path));
    
    public bool ContainsScene(string path) => TryContains(GetScenePath(path));
    
    public bool ContainsAudio(string path) => TryContains(GetAudioPath(path));
    
    public bool ContainsPath(string path) => TryContains(path);

    public string GetResourcePath(string path) => GetPathWithExt(path, "res");
    
    public string GetScenePath(string path) => GetPathWithExt(path, "scn");
    
    public string GetAudioPath(string path)
    {
        bool oggExists = _paths.Contains(path + ".ogg") || ResourceLoader.Exists(path + ".ogg");
        bool wavExists = _paths.Contains(path + ".wav") || ResourceLoader.Exists(path + ".wav");
        bool mp3Exists = _paths.Contains(path + ".mp3") || ResourceLoader.Exists(path + ".mp3");

        if (!oggExists && !wavExists && !mp3Exists)
            return "";
        
        if (oggExists)
            return path + ".ogg";
        
        if (wavExists)
            return path + ".wav";
        
        return path + ".mp3";
    }

    public IEnumerator<string> GetEnumerator() => _paths.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _paths.Count;

    public string this[int index] => _paths[index];

    private void TryAdd(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;
        
        _paths.Add(path);
    }

    private void TryRemove(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

        _paths.Remove(path);
    }

    private bool TryContains(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;
        
        return _paths.Contains(path);
    }
    
    private string GetPathWithExt(string path, string extension)
    {
        bool tExtExists = _paths.Contains(path + ".t" + extension) || ResourceLoader.Exists(path + ".t" + extension);
        bool extExists = _paths.Contains(path + "." + extension) || ResourceLoader.Exists(path + "." + extension);
        if (!tExtExists && !extExists)
            return "";
				
        if (tExtExists)
            return path + ".t" + extension;
        
        return path + "." + extension;
    }
}