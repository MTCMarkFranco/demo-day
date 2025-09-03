# Demo Day StreamingMarkdown iOS App - Agentic Rebuild Prompt

Create a complete iOS application that streams real-time Markdown content from a backend API with character-by-character rendering using SwiftUI and modern Swift concurrency. Use the following specifications to rebuild the entire iOS project from scratch.

## Project Overview
Build a streaming Markdown viewer iOS app that connects to the Demo Day AI Agent API and displays responses in real-time with smooth auto-scrolling and user interaction capabilities.

## CLI Commands to Execute

```bash
# Navigate to iOS workspace
cd iOS

# Remove existing project (if rebuilding)
rm -rf StreamingMarkdown.xcodeproj StreamingMarkdown/

# Create new iOS project directory structure
mkdir -p StreamingMarkdown/Assets.xcassets/AppIcon.appiconset
mkdir -p StreamingMarkdown/Preview\ Content
mkdir -p StreamingMarkdown/StreamingMarkdown/Base.lproj

# Alternative: Use Xcode CLI to create the project (requires Xcode Command Line Tools)
# Note: This creates the basic structure, but manual file creation is more reliable for automation
```

## Project Structure Requirements

```
iOS/
├── README.md                                    # iOS project documentation
├── StreamingMarkdown.xcodeproj/                 # Xcode project file
│   ├── project.pbxproj                         # Project configuration
│   └── project.xcworkspace/                    # Workspace configuration
└── StreamingMarkdown/                           # Source code directory
    ├── StreamingMarkdownApp.swift              # App entry point with SwiftUI
    ├── ContentView.swift                       # Main UI view with streaming display
    ├── StreamingViewModel.swift                # MVVM view model with @Published properties
    ├── StreamingService.swift                  # Network service with URLSession streaming
    ├── logo.svg                                # App logo (SVG format)
    ├── Assets.xcassets/                        # Asset catalog
    │   └── AppIcon.appiconset/                 # App icon assets
    │       └── Contents.json                   # Icon configuration
    ├── Preview Content/                        # SwiftUI preview assets
    │   └── Preview Assets.xcassets/            # Preview-only assets
    └── StreamingMarkdown/                      # Additional resources
        ├── Info.plist                         # App configuration (if needed)
        └── Base.lproj/                         # Localization resources
            └── LaunchScreen.storyboard         # Launch screen
```

## Core Implementation Requirements

### 1. App Entry Point

**File: StreamingMarkdownApp.swift**
```swift
import SwiftUI

@main
struct StreamingMarkdownApp: App {
    @StateObject private var viewModel = StreamingViewModel()
    var body: some Scene {
        WindowGroup {
            ContentView()
                .environmentObject(viewModel)
        }
    }
}
```

### 2. Main User Interface

**File: ContentView.swift**
- Create SwiftUI view with ScrollView and auto-scroll capability
- Use `ScrollViewReader` with proxy for programmatic scrolling
- Implement `TextField` with multi-line support (axis: .vertical, lineLimit: 1...4)
- Add send button with loading state (ProgressView when streaming)
- Use `MarkdownUI` library for rendering with `.markdownTheme(.gitHub)`
- Include placeholder text: "### Ask a question to start streaming..."
- Implement smooth scrolling animation: `.easeOut(duration: 0.25)`
- Disable send button when query is empty or streaming is active
- Use `@EnvironmentObject` to access `StreamingViewModel`
- Handle markdown text changes with `onChange` modifier for auto-scroll
- Include proper spacing and padding for iPhone layouts

### 3. View Model (MVVM Pattern)

**File: StreamingViewModel.swift**
- Implement `@MainActor` class conforming to `ObservableObject`
- Published properties:
  - `@Published var markdownText: String = ""`
  - `@Published var currentQuery: String = ""`
  - `@Published var isStreaming: Bool = false`
- Private properties:
  - `private let service = StreamingService()`
  - `private var streamTask: Task<Void, Never>? = nil`
- Methods:
  - `func sendCurrentQuery()` - validates and initiates streaming
  - `func startStreaming(query: String)` - manages streaming lifecycle
- Features:
  - Append user query to markdown with "**You:** {query}" format
  - Handle task cancellation for new requests
  - Use Swift structured concurrency with `Task` and `async/await`
  - Append streamed tokens to `markdownText` on `MainActor`
  - Handle errors gracefully with "**Error:** {error}" format
  - Reset `isStreaming` state after completion or error

### 4. Networking Service

**File: StreamingService.swift**
- Create `struct StreamingService` with streaming capabilities
- Define custom error enum: `StreamError` with cases: `.badURL`, `.http(Int)`, `.noData`
- Implement `func streamResponse(for query: String) async throws -> AsyncThrowingStream<String, Error>`
- Use `URLComponents` for safe URL construction with query parameters
- Configure `URLRequest` with:
  - HTTP method: "GET"
  - Accept header: "text/event-stream"
- Use `URLSession.shared.bytes(for:)` for streaming response
- Handle HTTP status codes ≥ 400 as errors
- Convert bytes to UTF-8 characters byte-by-byte
- Use `AsyncThrowingStream` with continuation for character streaming
- Target API endpoint: "https://demo-day-api-app.azurewebsites.net/api/agent/stream"
- Implement proper error handling and cleanup

### 5. Swift Package Dependencies

**Package Requirements:**
- **MarkdownUI**: https://github.com/gonzalezreal/swift-markdown-ui
  - Version: 2.4.0+ (up to next major version)
  - Purpose: Rendering streamed Markdown content
  - Usage: `import MarkdownUI`, `.markdownTheme(.gitHub)`

### 6. Xcode Project Configuration

**File: project.pbxproj (Key Settings)**
```
Build Settings:
- iOS Deployment Target: 17.5
- Swift Language Version: 5.0
- Product Bundle Identifier: com.example.StreamingMarkdown
- Marketing Version: 1.0
- Current Project Version: 1
- Targeted Device Family: iPhone & iPad (1,2)
- Code Sign Style: Automatic
- Development Team: YOURTEAMID (replace with actual)

Capabilities:
- SwiftUI App Lifecycle
- Automatic Info.plist generation
- SwiftUI Previews enabled
- Launch Screen generation

Supported Orientations:
- iPhone: Portrait, Landscape Left, Landscape Right
- iPad: All orientations including Portrait Upside Down
```

### 7. Asset Configuration

**File: Assets.xcassets/AppIcon.appiconset/Contents.json**
```json
{
  "images" : [
    {"idiom":"iphone","size":"20x20","scale":"2x","filename":"Icon-20@2x.png"},
    {"idiom":"iphone","size":"20x20","scale":"3x","filename":"Icon-20@3x.png"},
    {"idiom":"iphone","size":"29x29","scale":"2x","filename":"Icon-29@2x.png"},
    {"idiom":"iphone","size":"29x29","scale":"3x","filename":"Icon-29@3x.png"},
    {"idiom":"iphone","size":"40x40","scale":"2x","filename":"Icon-40@2x.png"},
    {"idiom":"iphone","size":"40x40","scale":"3x","filename":"Icon-40@3x.png"},
    {"idiom":"iphone","size":"60x60","scale":"2x","filename":"Icon-60@2x.png"},
    {"idiom":"iphone","size":"60x60","scale":"3x","filename":"Icon-60@3x.png"},
    {"idiom":"ipad","size":"20x20","scale":"1x","filename":"Icon-20.png"},
    {"idiom":"ipad","size":"20x20","scale":"2x","filename":"Icon-20@2x-1.png"},
    {"idiom":"ipad","size":"29x29","scale":"1x","filename":"Icon-29.png"},
    {"idiom":"ipad","size":"29x29","scale":"2x","filename":"Icon-29@2x-1.png"},
    {"idiom":"ipad","size":"40x40","scale":"1x","filename":"Icon-40.png"},
    {"idiom":"ipad","size":"40x40","scale":"2x","filename":"Icon-40@2x-1.png"},
    {"idiom":"ipad","size":"76x76","scale":"1x","filename":"Icon-76.png"},
    {"idiom":"ipad","size":"76x76","scale":"2x","filename":"Icon-76@2x.png"},
    {"idiom":"ipad","size":"83.5x83.5","scale":"2x","filename":"Icon-83.5@2x.png"},
    {"idiom":"ios-marketing","size":"1024x1024","scale":"1x","filename":"Icon-1024.png"}
  ],
  "info" : { "version" : 1, "author" : "xcode" }
}
```

### 8. App Icon Generation

**CLI Commands for Icon Generation (requires ImageMagick):**
```bash
# Install ImageMagick (macOS)
brew install imagemagick

# Generate all required icon sizes from logo.svg
magick convert logo.svg -resize "20x20" -background none -gravity center -extent "20x20" "Assets.xcassets/AppIcon.appiconset/Icon-20.png"
magick convert logo.svg -resize "40x40" -background none -gravity center -extent "40x40" "Assets.xcassets/AppIcon.appiconset/Icon-20@2x.png"
magick convert logo.svg -resize "60x60" -background none -gravity center -extent "60x60" "Assets.xcassets/AppIcon.appiconset/Icon-20@3x.png"
magick convert logo.svg -resize "29x29" -background none -gravity center -extent "29x29" "Assets.xcassets/AppIcon.appiconset/Icon-29.png"
magick convert logo.svg -resize "58x58" -background none -gravity center -extent "58x58" "Assets.xcassets/AppIcon.appiconset/Icon-29@2x.png"
magick convert logo.svg -resize "87x87" -background none -gravity center -extent "87x87" "Assets.xcassets/AppIcon.appiconset/Icon-29@3x.png"
magick convert logo.svg -resize "40x40" -background none -gravity center -extent "40x40" "Assets.xcassets/AppIcon.appiconset/Icon-40.png"
magick convert logo.svg -resize "80x80" -background none -gravity center -extent "80x80" "Assets.xcassets/AppIcon.appiconset/Icon-40@2x.png"
magick convert logo.svg -resize "120x120" -background none -gravity center -extent "120x120" "Assets.xcassets/AppIcon.appiconset/Icon-40@3x.png"
magick convert logo.svg -resize "120x120" -background none -gravity center -extent "120x120" "Assets.xcassets/AppIcon.appiconset/Icon-60@2x.png"
magick convert logo.svg -resize "180x180" -background none -gravity center -extent "180x180" "Assets.xcassets/AppIcon.appiconset/Icon-60@3x.png"
magick convert logo.svg -resize "76x76" -background none -gravity center -extent "76x76" "Assets.xcassets/AppIcon.appiconset/Icon-76.png"
magick convert logo.svg -resize "152x152" -background none -gravity center -extent "152x152" "Assets.xcassets/AppIcon.appiconset/Icon-76@2x.png"
magick convert logo.svg -resize "167x167" -background none -gravity center -extent "167x167" "Assets.xcassets/AppIcon.appiconset/Icon-83.5@2x.png"
magick convert logo.svg -resize "1024x1024" -background none -gravity center -extent "1024x1024" "Assets.xcassets/AppIcon.appiconset/Icon-1024.png"

# Generate duplicates for iPad-specific variants
cp "Assets.xcassets/AppIcon.appiconset/Icon-20@2x.png" "Assets.xcassets/AppIcon.appiconset/Icon-20@2x-1.png"
cp "Assets.xcassets/AppIcon.appiconset/Icon-29@2x.png" "Assets.xcassets/AppIcon.appiconset/Icon-29@2x-1.png"
cp "Assets.xcassets/AppIcon.appiconset/Icon-40@2x.png" "Assets.xcassets/AppIcon.appiconset/Icon-40@2x-1.png"
```

## Technical Specifications

### Swift Concurrency Implementation
- Use `@MainActor` for UI-related classes to ensure main thread execution
- Implement `AsyncThrowingStream<String, Error>` for character streaming
- Use `Task` with proper cancellation handling via `CancellationToken`
- Employ `async/await` patterns throughout the networking layer
- Handle stream completion and error states gracefully

### SwiftUI Best Practices
- Use `@StateObject` for view model initialization in App
- Use `@EnvironmentObject` for dependency injection to child views
- Implement `@Published` properties for reactive UI updates
- Use `ScrollViewReader` for programmatic scroll control
- Implement proper `onChange` modifiers for side effects
- Use `.id()` modifier for ScrollView anchor targeting

### Networking Architecture
- Use `URLSession.shared.bytes(for:)` for HTTP streaming
- Implement proper URL encoding with `URLComponents`
- Handle HTTP status codes and network errors appropriately
- Convert byte streams to character streams for UI consumption
- Use structured concurrency for request lifecycle management

### UI/UX Requirements
- Smooth auto-scrolling to latest content with animation
- Responsive layout for all iPhone and iPad sizes
- Loading states with progress indicators
- Input validation and button state management
- GitHub-themed Markdown rendering for consistency
- Multi-line text input with appropriate height constraints

### Error Handling
- Network error handling with user-friendly messages
- Stream cancellation support for new requests
- Graceful degradation for connection issues
- Proper cleanup of background tasks and streams

## Development Workflow Commands

```bash
# Open project in Xcode
open StreamingMarkdown.xcodeproj

# Build from command line (optional)
xcodebuild -project StreamingMarkdown.xcodeproj -scheme StreamingMarkdown -configuration Debug

# Clean build folder
xcodebuild clean -project StreamingMarkdown.xcodeproj -scheme StreamingMarkdown

# Run tests (if any)
xcodebuild test -project StreamingMarkdown.xcodeproj -scheme StreamingMarkdown -destination 'platform=iOS Simulator,name=iPhone 15'
```

## Expected Features After Implementation

1. **Real-time Streaming**: Character-by-character display of AI responses
2. **Auto-scrolling**: Smooth scroll to latest content as it arrives
3. **User Input**: Multi-line text field with send button
4. **Loading States**: Progress indicator during streaming
5. **Error Handling**: Graceful error display and recovery
6. **Markdown Rendering**: Rich text display with GitHub theme
7. **Responsive Design**: Works on all iPhone and iPad sizes
8. **Modern Architecture**: MVVM with SwiftUI and Swift concurrency

## Success Criteria

✅ Project builds without errors or warnings in Xcode  
✅ App launches successfully on iOS Simulator  
✅ Streaming works character-by-character from API  
✅ Auto-scrolling functions smoothly during streaming  
✅ Markdown rendering displays properly formatted content  
✅ User input and send button work correctly  
✅ Loading states and error handling function as expected  
✅ App icons display correctly in all required sizes  
✅ Swift concurrency patterns work without data races  
✅ Memory management is efficient with no leaks  

## Development Notes

### Xcode Project Creation (Manual Alternative)
If CLI project creation is not available, create the Xcode project manually:
1. Open Xcode
2. Create new iOS App project
3. Choose SwiftUI interface and Swift language
4. Set bundle identifier to `com.example.StreamingMarkdown`
5. Enable "Use Swift Package Manager" if prompted
6. Add MarkdownUI package dependency

### Swift Package Manager Integration
Add the MarkdownUI package through Xcode:
1. File → Add Package Dependencies
2. Enter URL: `https://github.com/gonzalezreal/swift-markdown-ui`
3. Choose "Up to Next Major Version" with minimum 2.4.0
4. Add MarkdownUI to app target

### Testing Strategy
- Test on multiple iPhone sizes (SE, standard, Plus/Pro Max)
- Test on iPad in both orientations
- Verify streaming works with various query lengths
- Test network error scenarios (airplane mode, bad connectivity)
- Validate memory usage during long streaming sessions

## Notes for Agentic Implementation

- Follow exact Swift naming conventions and coding standards
- Implement all async/await patterns correctly to avoid data races
- Use proper SwiftUI state management with @Published/@StateObject
- Ensure proper memory management and avoid retain cycles
- Test streaming functionality thoroughly with real API calls
- Validate UI responsiveness during streaming operations
- Implement proper accessibility support for VoiceOver users
- Follow Apple Human Interface Guidelines for iOS apps
- Ensure app works correctly on all supported iOS versions (17.5+)

This prompt provides complete specifications to rebuild the StreamingMarkdown iOS app from scratch using modern Swift development practices, SwiftUI, and Swift concurrency features.
