import SwiftUI
import MarkdownUI

struct ContentView: View {
    @EnvironmentObject private var viewModel: StreamingViewModel
    @State private var scrollToBottom: Bool = false

    var body: some View {
        VStack(spacing: 0) {
            ScrollViewReader { proxy in
                ScrollView {
                    Markdown(viewModel.markdownText.isEmpty ? placeholder : viewModel.markdownText)
                        .markdownTheme(.gitHub)
                        .padding(.horizontal)
                        .id("BOTTOM")
                }
                .background(Color(UIColor.systemBackground))
                .onChange(of: viewModel.markdownText) { _ in
                    withAnimation(.easeOut(duration: 0.25)) {
                        proxy.scrollTo("BOTTOM", anchor: .bottom)
                    }
                }
            }
            Divider()
            HStack(alignment: .bottom) {
                TextField("Ask something…", text: $viewModel.currentQuery, axis: .vertical)
                    .textFieldStyle(.roundedBorder)
                    .lineLimit(1...4)
                Button(action: viewModel.sendCurrentQuery) {
                    if viewModel.isStreaming { ProgressView() } else { Image(systemName: "paperplane.fill") }
                }
                .disabled(viewModel.currentQuery.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty || viewModel.isStreaming)
            }
            .padding()
        }
    }

    private var placeholder: String { "### Ask a question to start streaming..." }
}

#Preview {
    ContentView().environmentObject(StreamingViewModel())
}
