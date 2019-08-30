# Cake.WinSCP
Cake AddIn to upload files using WinSCP.

[![Nuget](https://img.shields.io/nuget/vpre/Cake.WinSCP.svg?label=Cake.WinSCP)](https://www.nuget.org/packages/Cake.WinSCP/)
![AppVeyor](https://img.shields.io/appveyor/ci/cakecontrib/cake-winscp)

## Example usage

### Basic Usage
```cs
#addin Cake.WinSCP

WinScpSync(
    "ftp://username:password@site.com/",
    "/public",
    @"c:\projects\site",
    false
);
```

### Advanced Usage

Advanced method for finer control of directory synchronization

```cs
#addin Cake.WinSCP

WinScpSync(
    new WinSCP.SessionOptions(
        Protocol = WinSCP.Protocol.Ftp,
        HostName = "site.com",
        UserName = "username",
        Password = "mypassword"
    ),
    "/public",
    @"c:\projects\site",
    false,
    WinSCP.SynchronizationMode.Remote,
    false,
    WinSCP.SynchronizationCriteria.Time,
    new WinSCP.TransferOptions()
);
```
See https://winscp.net/eng/docs/library for more details about `SessionOptions`, `TransferOptions` and the other settings

### Pushing Files

Push files from local to remote location

```cs
#addin Cake.WinSCP

WinScpPut(
    new WinSCP.SessionOptions(
        Protocol = WinSCP.Protocol.Ftp,
        HostName = "site.com",
        UserName = "username",
        Password = "mypassword"
    ),
    "/public",
    @"c:\projects\site",
    false,
    new WinSCP.TransferOptions()
);
```

### Listing Files

List the files that exist at the given remote location

```cs
#addin Cake.WinSCP

var results = WinScpList(
    new WinSCP.SessionOptions(
        Protocol = WinSCP.Protocol.Ftp,
        HostName = "site.com",
        UserName = "username",
        Password = "mypassword"
    ),
    "/public"
);

foreach(var result in results) 
{
    Console.WriteLine(result.Name);
}

```

### Get Files

Get the files at the remote location and download them to the local directory

```cs
#addin Cake.WinSCP

WinScpGet(
    new WinSCP.SessionOptions(
        Protocol = WinSCP.Protocol.Ftp,
        HostName = "site.com",
        UserName = "username",
        Password = "mypassword"
    ),
    "/public",
    @"c:\projects\site",
    false,
    new WinSCP.TransferOptions()
);
```

### Compare Files

Compare the local and remote directories without synchronizing them

```cs
#addin Cake.WinSCP

var results = WinScpCompare(
    new WinSCP.SessionOptions(
        Protocol = WinSCP.Protocol.Ftp,
        HostName = "site.com",
        UserName = "username",
        Password = "mypassword"
    ),
    "/public",
    @"c:\projects\site",
    false,
    false,
    WinSCP.SynchronizationMode.Remote,
    false,
    WinSCP.SynchronizationCriteria.Time,
    new WinSCP.TransferOptions()
);

foreach(var result in results) 
{
    Console.WriteLine(result.Action);
    Console.WriteLine(result.IsDirectory);
    Console.WriteLine(result.Local);
    Console.WriteLine(result.Remote);
}

```

