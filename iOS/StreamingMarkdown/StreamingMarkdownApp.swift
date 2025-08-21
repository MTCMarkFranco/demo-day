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
