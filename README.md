# Cake.WinSCP
Cake AddIn to upload files using WinSCP.

## Example usage
```cs
#addin Cake.WinSCP

WinScpSync(
    "ftp://username:password@site.com/",
    "/public",
    @"c:\projects\site",
    false
);
```