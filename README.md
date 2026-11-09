## Development

Resume Rewind is built using:

- C#
- .NET 10
- Jellyfin Plugin SDK / API
- Newtonsoft.Json
- GitHub Actions

### Requirements

- Git
- .NET 10 SDK
- Visual Studio, Visual Studio Code, or another C# editor

### Clone the Repository

```powershell
git clone https://github.com/byteseekersocial/Resume-Rewind.git
cd Resume-Rewind
```

### Branches

| Branch | Purpose |
|---|---|
| `testing` | Development and testing releases |
| `latest` | Stable releases |

New features should be developed and tested on `testing` before being promoted to `latest`.

## Creating a New Release

The following instructions describe the manual release process using Windows PowerShell.

Replace `VERSION` with the new version number throughout these instructions. For example, `0.1.2.6`.

### 1. Update the Version

Before building, update the plugin version in the project file:

`Jellyfin.Plugin.Template/Jellyfin.Plugin.Template.csproj`

Update the package, assembly, and file version values to the new version.

Commit and push your changes to the appropriate branch (`testing` or `latest`).

### 2. Build the Plugin

From the repository's root directory, run:

```powershell
dotnet build .\Jellyfin.Plugin.Template\Jellyfin.Plugin.Template.csproj -c Release
```

Make sure the build succeeds before continuing.

### 3. Create the Release ZIP

Run:

```powershell
$version = "VERSION"
$source = ".\Jellyfin.Plugin.Template\bin\Release\net10.0"
$destination = ".\release\resume-rewind_$version.zip"

New-Item -ItemType Directory -Force -Path ".\release" | Out-Null

Compress-Archive -Path `
    "$source\ResumeRewind.dll", `
    "$source\ResumeRewind.deps.json", `
    "$source\ResumeRewind.pdb", `
    "$source\ResumeRewind.xml" `
    -DestinationPath $destination -Force
```

The ZIP should contain:

```text
ResumeRewind.dll
ResumeRewind.deps.json
ResumeRewind.pdb
ResumeRewind.xml
```

### 4. Verify the ZIP

Check its contents:

```powershell
tar -tf ".\release\resume-rewind_VERSION.zip"
```

Calculate its MD5 checksum:

```powershell
Get-FileHash ".\release\resume-rewind_VERSION.zip" -Algorithm MD5
```

Copy the resulting checksum. You will need it for the plugin manifest.

### 5. Update the Manifest

Open the manifest for the branch you are releasing:

- Stable: `manifest.json` on `latest`.
- Testing: `manifest.json` on `testing`.

Update the existing release entry with:

- New version number.
- New ZIP download URL.
- New MD5 checksum.
- Current UTC timestamp.

Generate a timestamp using PowerShell:

```powershell
(Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
```

The ZIP download URL follows this format:

```text
https://github.com/byteseekersocial/Resume-Rewind/releases/download/vVERSION/resume-rewind_VERSION.zip
```

For a testing release, use the corresponding testing release tag instead.

Keep the manifest's existing JSON structure and update the appropriate release entry. Make sure the JSON remains valid.

### 6. Commit and Push the Manifest

After updating the manifest:

```powershell
git add manifest.json
git commit -m "Update manifest for VERSION"
git push origin BRANCH
```

Replace `BRANCH` with `latest` or `testing`.

### 7. Create a GitHub Release

1. Open the [Resume Rewind GitHub Releases](https://github.com/byteseekersocial/Resume-Rewind/releases) page.
2. Select **Draft a new release**.
3. Create a new tag:
   - Stable: `vVERSION`
   - Testing: `testing-vVERSION`
4. Select the appropriate target branch.
5. Enter the release title, such as `Resume Rewind VERSION`.
6. Add release notes describing the changes.
7. Upload `resume-rewind_VERSION.zip`.
8. For testing releases, mark the release as a pre-release.
9. For stable releases, leave the pre-release option unchecked.
10. Publish the release.

### 8. Verify the Release

After publishing:

1. Open the release page and make sure the ZIP is attached.
2. Open the branch's raw `manifest.json` URL.
3. Verify that the new version, download URL, checksum, and timestamp are correct.
4. Refresh the Jellyfin plugin catalog and check that the new version appears.

**Important:** Publish the GitHub release and upload the ZIP before relying on the manifest's download URL. Otherwise, Jellyfin may try to download a file that does not exist yet.
