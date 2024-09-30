# School Management System

The School Management System is a C# application designed to manage various aspects of a school, including students, teachers, and courses. This system allows for the demonstration of actions performed by teachers and students, as well as the management of course enrollments and student grades.

## Features

- **Teacher Actions**: Teach, check attendance, grade papers, prepare lessons, and attend meetings.
- **Student Actions**: Learn, take tests, submit assignments, study, and participate in class.
- **Course Management**: Enroll and unenroll students, update course credits, and display student grades.
- **Interactive Console**: User-friendly console interface for managing students and courses interactively.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [JetBrains Rider](https://www.jetbrains.com/rider/) (recommended IDE)

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/SchoolManagementSystem.git
    ```
2. Navigate to the project directory:
    ```sh
    cd SchoolManagementSystem
    ```
3. Open the project in JetBrains Rider.

### Usage

1. Build the project:
    ```sh
    dotnet build
    ```
2. Run the project:
    ```sh
    dotnet run
    ```

### Project Structure

- `SchoolManagementSystem/Interfaces`: Contains interface definitions for actions performed by teachers and students.
- `SchoolManagementSystem/PresentationLayer/Handlers`: Contains handler classes for managing courses and students.
- `SchoolManagementSystem/Models`: Contains model classes representing students, teachers, and courses.

### Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes.

### License

This project is licensed under the MIT License. See the `LICENSE` file for more details.

### Contact

For any questions or suggestions, please open an issue on GitHub.
