import Foundation

struct StreamingService {
    enum StreamError: Error { case badURL, http(Int), noData }

    func streamResponse(for query: String) async throws -> AsyncThrowingStream<String, Error> {
        guard var components = URLComponents(string: "https://demo-day-api-app.azurewebsites.net/api/agent/stream") else {
            throw StreamError.badURL
        }
        components.queryItems = [URLQueryItem(name: "query", value: query)]
        guard let url = components.url else { throw StreamError.badURL }

        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        request.setValue("text/event-stream", forHTTPHeaderField: "Accept")
        let (bytes, response) = try await URLSession.shared.bytes(for: request)
        if let http = response as? HTTPURLResponse, http.statusCode >= 400 {
            throw StreamError.http(http.statusCode)
        }
        var iterator = bytes.makeAsyncIterator()
        return AsyncThrowingStream { continuation in
            Task {
                do {
                    while let byte = try await iterator.next() {
                        if let scalar = String(bytes: [byte], encoding: .utf8) {
                            continuation.yield(scalar)
                        }
                    }
                    continuation.finish()
                } catch {
                    continuation.finish(throwing: error)
                }
            }
        }
    }
}
