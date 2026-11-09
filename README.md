# Resume Rewind

A Jellyfin plugin that automatically rewinds playback when resuming movies and TV episodes.

## Websites

- Go to our [website](https://resume-rewind.byteseeker.cc) to see everything without straining your eyes.
- Go to [this site](https://rr.byteseeker.cc) to see this README better.

## Features

- Automatically rewinds playback when resuming.
- Separate settings for movies and TV episodes.
- Administrator defaults and individual user preferences.
- Configurable rewind duration from 0 to 60 seconds.
- Reset individual preferences to administrator defaults.
- Works server-side without requiring client modifications.

## Installation

1. Open the Jellyfin Dashboard.
2. Navigate to **Plugins → Repositories**.
3. Add a repository:

   **Repository Name:** Resume Rewind

   **Repository URL:**
   ```text
   https://raw.githubusercontent.com/byteseekersocial/Resume-Rewind/latest/manifest.json
   ```

4. Save the repository and refresh the plugin catalog.
5. Find **Resume Rewind** and install it.
6. Restart Jellyfin.

### Testing Releases

To install development releases, add another repository:

**Repository Name:** Resume Rewind Testing

**Repository URL:**
```text
https://raw.githubusercontent.com/byteseekersocial/Resume-Rewind/testing/manifest.json
```

Testing releases may contain unfinished features or bugs.

## Administrator Instructions

Administrators can configure the default rewind settings for the entire server.

1. Open the Jellyfin Dashboard.
2. Navigate to **Plugins → My Plugins**.
3. Select **Resume Rewind**.
4. Open the configuration page.
5. Configure the following settings:

| Setting | Description |
|---|---|
| Enable Episode Rewind | Enables or disables episode rewinding by default. |
| Episode Rewind Seconds | Number of seconds to rewind episodes. |
| Enable Movie Rewind | Enables or disables movie rewinding by default. |
| Movie Rewind Seconds | Number of seconds to rewind movies. |

6. Save the configuration.

Settings apply to users who have not customized their individual preferences.

## User Instructions

Users can customize their own rewind settings without changing the administrator's defaults.

1. Open Jellyfin.
2. Navigate to the **Resume Rewind** preferences page.
3. Configure your preferences:

**TV Episodes**
- Enable or disable episode rewinding.
- Choose the number of seconds to rewind.

**Movies**
- Enable or disable movie rewinding.
- Choose the number of seconds to rewind.

4. Save your preferences.

### Resetting Preferences

Each media type has its own reset option.

- Resetting episode preferences restores the administrator's episode defaults.
- Resetting movie preferences restores the administrator's movie defaults.

Your preferences are saved separately for your Jellyfin account and persist across server restarts.

## How It Works

When Jellyfin saves your playback position, Resume Rewind adjusts that position by subtracting your configured rewind duration.

For example, if you stop watching at 15:30 and have a 10-second rewind configured, the next playback will resume at approximately 15:20.

Individual user preferences override administrator defaults. If you have not customized a media type, the administrator's settings are used.

## Client Compatibility

| Jellyfin Client | Status |
|---|---|
| Jellyfin Web | Confirmed working |
| Roku | Confirmed working |
| Android TV | Not yet tested |
| Android Mobile | Confirmed working |
| iOS | Confirmed working |
| Swiftfin | Confirmed working |
| Xbox client | Confirmed working |
| Tizen TV | Not yet tested |
| WebOS Client | Not yet tested |
| Google TV | Not yet tested |
| Desktop clients | Confirmed working |
| Other clients | Not yet tested |

Compatibility testing is ongoing. Please report any issues or successful tests!

## Troubleshooting

**Playback does not rewind:**
- Make sure Resume Rewind is installed and enabled.
- Check that rewinding is enabled for the media type.
- Make sure the rewind duration is greater than zero.
- Check whether your individual preferences override the administrator's settings.

**Preferences page says "Page not found":**
- Make sure the Plugin Pages plugin is installed and enabled.
- Try opening the page through Jellyfin Web.
- Restart the Jellyfin application and try again.

**Still having problems?**

Open an issue on GitHub and include your Jellyfin version, Resume Rewind version, affected client, and relevant server logs.

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

## Contributing

Contributions, bug reports, feature requests, and compatibility testing are welcome!

If you would like to contribute, open an issue or contact me through GitHub.

Repository: https://github.com/byteseekersocial/Resume-Rewind

## Roadmap

- [x] Initial plugin implementation.
- [x] Separate movie and episode settings.
- [x] Jellyfin Web compatibility.
- [x] Roku compatibility.
- [x] Automated release workflow.
- [x] Per-user rewind preferences.
- [x] Reset preferences to administrator defaults.
- [ ] Additional client compatibility testing.
- [ ] More configuration options.

## License

See [LICENSE](LICENSE) for licensing information.

---

**Resume Rewind is an independent community project and is not an official Jellyfin project.**
