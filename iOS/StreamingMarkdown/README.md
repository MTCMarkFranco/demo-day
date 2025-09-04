# StreamingMarkdown iOS App

Real-time streaming Markdown viewer using SwiftUI + MarkdownUI.

## Features
- Streams markdown tokens from backend and updates live.
- Smooth auto-scroll to latest content.
- User input field with send button.
- Uses `URLSession.bytes` + `AsyncThrowingStream` for character/token streaming.
- Swift concurrency throughout (`Task`, `AsyncStream`, `MainActor`).
- Adaptive layout for all iPhone sizes.
- New changes

## Requirements
- Xcode 15+
- iOS 17+ (adjust deployment target if needed)

## Setup (Ready-to-Run)
1. Open `StreamingMarkdown.xcodeproj` in Xcode 15+.
2. Xcode should automatically resolve the `MarkdownUI` Swift Package (declared in the project). If prompted, allow it.
3. Replace `YOURTEAMID` in the project Signing settings with your Apple Developer Team ID.
4. Select an iPhone simulator (or device) and Build & Run.

## How Streaming Works
`StreamingService` uses `URLSession.shared.bytes(for:)` to obtain an `AsyncSequence` of bytes. Bytes are converted to UTF-8 characters and yielded via `AsyncThrowingStream<String, Error>`. The `StreamingViewModel` subscribes and appends tokens to the published `markdownText`, triggering reactive UI updates.

## Sample Endpoint
The demo endpoint is called like:
```
https://demo-day-api-app.azurewebsites.net/api/agent/stream?query=Hello%2C%20what%20is%20the%20weather%20like%20today%20in%20Toronto%3F
```
Queries are URL encoded automatically.

## Icon & Branding
`logo.svg` provided. Export PNG sizes for AppIcon using the script below (requires PowerShell + ImageMagick's `magick`) — many are already referenced in `Contents.json`; generate actual PNGs before archiving to App Store.

### Export Script (PowerShell)
```powershell
$src = "logo.svg"
$map = @{
  "Icon-20"=20; "Icon-20@2x"=40; "Icon-20@3x"=60;
  "Icon-29"=29; "Icon-29@2x"=58; "Icon-29@3x"=87;
  "Icon-40"=40; "Icon-40@2x"=80; "Icon-40@3x"=120;
  "Icon-60@2x"=120; "Icon-60@3x"=180;
  "Icon-76"=76; "Icon-76@2x"=152;
  "Icon-83.5@2x"=167;
  "Icon-1024"=1024
}
$dest = "Assets.xcassets/AppIcon.appiconset"
foreach ($k in $map.Keys) { magick convert $src -resize "$($map[$k])x$($map[$k])" -background none -gravity center -extent "$($map[$k])x$($map[$k])" "$dest/$k.png" }
```
Ensure transparency is preserved. Then verify in Xcode's asset catalog.

## Future Enhancements
- Support multi-turn conversation with role separation.
- Token-level animations/highlights.
- Cancellable streams & retry.
- Offline caching of previous sessions.

## License
MIT (adjust as needed)
