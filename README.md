# Demo Day Project

Welcome to the Demo Day project! This repository contains both backend API and iOS mobile application components.

## Project Structure

```
demo-day/
├── api/          # Backend API server
├── iOS/          # iOS mobile application
└── README.md     # This file
```

## Getting Started

### Prerequisites

- Node.js (for API development)
- Xcode (for iOS development)
- Git

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/MTCMarkFranco/demo-day.git
   cd demo-day
   ```

2. Set up the API:
   ```bash
   cd api
   # Install dependencies (adjust based on your tech stack)
   npm install
   ```

3. Set up the iOS app:
   ```bash
   cd ../iOS
   # Open the project in Xcode
   open *.xcodeproj
   ```

## API

The backend API provides the server-side functionality for the application.

### Running the API

```bash
cd api
npm start
```

The API will be available at `http://localhost:3000` (or your configured port).

## iOS App

The iOS application provides the mobile interface for users.

### Running the iOS App

1. Open the project in Xcode
2. Select your target device or simulator
3. Build and run the project (⌘+R)

## Development

### API Development

- Add your API routes and controllers in the `api/` directory
- Configure environment variables as needed
- Follow RESTful API conventions

### iOS Development

- Implement your view controllers and UI in the `iOS/` directory
- Use appropriate iOS design patterns (MVC, MVVM, etc.)
- Test on both simulator and physical devices

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Testing

### API Testing
```bash
cd api
npm test
```

### iOS Testing
Run tests in Xcode using ⌘+U or through the Test Navigator.

## Deployment

### API Deployment
- Configure your production environment
- Set up environment variables
- Deploy to your preferred hosting service

### iOS Deployment
- Configure signing certificates
- Build for release
- Submit to App Store Connect

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For questions or support, please contact the development team.

## Demo Day

This project was created for Demo Day presentation. The goal is to showcase [describe your project's main purpose and features here].

---

**Happy coding!** 🚀
