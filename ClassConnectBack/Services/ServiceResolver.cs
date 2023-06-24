using ClassConnect.Services.FileSystemServices.Helpers;

namespace ClassConnect.Services;

public delegate IFileSystemHelper ServiceResolver(Type key);
