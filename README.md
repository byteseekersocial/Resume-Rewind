# Resume Rewind

A Jellyfin plugin that rewinds playback when resuming movies and TV episodes.

## About

Resume Rewind was created as a personal project because I couldn't find another solution that provided the resume-rewind behavior I wanted in Jellyfin.

The goal is simple: when you resume watching something, automatically rewind a configurable number of seconds so you don't miss the end of the scene or anything that happened before you stopped watching.

This is an independent, community-based project. Contributions, suggestions, bug reports, and new ideas are always welcome!

**Anyone interested in becoming a contributor is welcome to message me.** You don't need to be an expert to get involved.

## Features

- Automatically rewinds playback when resuming.
- Separate settings for TV episodes and movies.
- Enable or disable rewind independently for movies and episodes.
- Configurable rewind duration.
- Server-side implementation, without requiring a separate client modification.

## Client Compatibility

Compatibility is still being tested.

The following clients are currently **known to work**:

| Jellyfin Client | Status |
|---|---|
| Jellyfin Web | Confirmed working |
| Roku | Confirmed working |
| Android TV | Not yet tested |
| Android Mobile | Not yet tested |
| iOS | Confirmed working |
| Desktop clients | Not yet tested |
| Other clients | Not yet tested |

Compatibility with untested clients is not guaranteed. Additional testing and community feedback are welcome!

## Configuration

Resume Rewind currently provides administrator-configurable settings for:

### TV Episodes
- Enable or disable episode rewind.
- Set the number of seconds to rewind.

### Movies
- Enable or disable movie rewind.
- Set the number of seconds to rewind.

The rewind duration can be configured between 0 and 60 seconds.

Per-user configuration is a planned improvement.

## Installation

Resume Rewind is distributed through a Jellyfin plugin repository.

1. Open your Jellyfin Dashboard.
2. Navigate to **Plugins → Repositories**.
3. Add the following repository:

   **Repository Name:** Resume Rewind Testing

   **Repository URL:**
   `https://raw.githubusercontent.com/byteseekersocial/Resume-Rewind/latest/manifest.json`
   **Testing URL:**
   `https://raw.githubusercontent.com/byteseekersocial/Resume-Rewind/testing/manifest.json`

5. Save the repository and refresh the plugin catalog.
6. Find **Resume Rewind** and install it.
7. Restart your Jellyfin server.
8. Open the Resume Rewind configuration page under your installed plugins.

> **Note:** The `testing` repository contains development releases. Features and behavior may change as development continues.

## Community & Contributions

Resume Rewind is a personal project, but it is also intended to be community-driven.

I welcome:
- Bug reports and troubleshooting help.
- Feature requests and suggestions.
- Testing on additional Jellyfin clients.
- Code contributions and improvements.
- Documentation improvements.

If you would like to help develop Resume Rewind or become a contributor, feel free to message me through GitHub.

You can also open an issue in this repository to report a problem or suggest an improvement.

Every bit of feedback helps!

## Development

Resume Rewind is built using:

- C#
- .NET 10
- Jellyfin Plugin SDK / API
- GitHub Actions for automated builds and releases

The `testing` branch is used for active development and testing. Once changes are stable, they will be moved into the `latest` branch for stable releases.

## Roadmap

- [x] Initial plugin implementation.
- [x] Separate movie and episode rewind settings.
- [x] Jellyfin Web compatibility.
- [x] Roku compatibility.
- [x] Automated GitHub release workflow.
- [ ] Per-user rewind preferences.
- [ ] Additional client compatibility testing.
- [ ] More configuration options.
- [ ] Stable release through the `latest` branch.

## License

See [LICENSE](LICENSE) for licensing information.

---

**Resume Rewind is an independent community project and is not an official Jellyfin project.**
