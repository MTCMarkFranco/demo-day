import Foundation
import Combine

@MainActor
final class StreamingViewModel: ObservableObject {
    @Published var markdownText: String = ""
    @Published var currentQuery: String = ""
    @Published var isStreaming: Bool = false

    private let service = StreamingService()
    private var streamTask: Task<Void, Never>? = nil

    func sendCurrentQuery() {
        let query = currentQuery.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !query.isEmpty else { return }
        currentQuery = ""
        startStreaming(query: query)
    }

    func startStreaming(query: String) {
        streamTask?.cancel()
        markdownText.append("\n\n**You:** \(query)\n\n")
        isStreaming = true
        streamTask = Task { [weak self] in
            guard let self else { return }
            do {
                let stream = try await service.streamResponse(for: query)
                for try await token in stream {
                    await MainActor.run {
                        self.markdownText.append(token)
                    }
                }
            } catch {
                await MainActor.run {
                    self.markdownText.append("\n\n**Error:** \(error.localizedDescription)\n")
                }
            }
            await MainActor.run { self.isStreaming = false }
        }
    }
}
