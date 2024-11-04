Blazor Chat Application


Overview
This is a Blazor chat application that allows users to interact by sending and receiving messages in real-time.
The application also includes a section where users can interact with a Llama model to ask questions or obtain information.


Features
Chat Functionality: Users can send and receive messages in a chat group.
User Selection: A dropdown menu allows users to choose who they want to be when sending messages.
Llama Interaction: Users can send prompts to a Llama model and receive responses.


Installation Instructions
To run the application locally, follow the steps below:


1. Clone the repository:
bash
Copy code
git clone <repository-url>
cd <repository-folder>

2. Install necessary packages: If you are using .NET CLI, run the following command to restore all dependencies:
bash
Copy code
dotnet restore

3. Start the application:
bash
Copy code
dotnet run

4. Open the browser: Navigate to http://localhost:5000 to see the application in action.


Components
ChatApp.razor
Description: This component handles the chat logic and displays messages sent by users.
Message Display: Loops through all messages and shows them with date, sender, and message text.
Input Field: Users can type a message and select a sender from a dropdown menu before sending the message.
Data Handling: Retrieves and saves messages to and from the database.


Llama.razor
Description: This component allows users to interact with the Llama model to obtain information or ask questions.
Prompt Input: Users can type a prompt and send it to the Llama model.
Result Display: Shows the response from the model in the application.


LlamaChat.razor
Description: This component integrates chat functionality with Llama interaction, allowing users to send prompts and see responses alongside chat messages.
Prompt Input: Users can type a prompt to the Llama model and send it directly.
Chat History: Displays a history of messages exchanged in the chat along with responses from the Llama model.


Installation
To install and run the LlamaChat component:

1. Clone the project to your local environment.
2. Ensure that all required dependencies are installed.
3. Start a local server to run the Blazor component with dotnet run.


Prerequisites
.NET 6.0 or later
A working Blazor Server setup
A running instance of the Llama API at http://localhost:11434/


Usage
LlamaChat allows users to send messages by typing into an input field and clicking the Send message button.
A dropdown menu enables the user to select a sender (Pippi, Tommy, or Annika). When a message is sent:
1. It is saved in a database.
2. A query is sent to the Llama API to receive a response, which is then displayed in the chat.


Commands and Controls
Text Input: Users type messages into the input field and press Send message to send.
Dropdown Menu: Users select a sender from a list of options.


Component Structure
LlamaChat.razor is made up of several core elements:

Message List: Iterates through mychat.Messages to display each message’s date, sender name, and text content.
Input Field: Allows the user to type a new message.
Sender Dropdown: Updates selectedUser based on the user’s choice.


Backend Code
The component uses C# code in the @code block to handle data loading, message sending and saving, and API requests.

Methods
OnInitialized
Executes when the component is first created. Initializes the user list and loads previous messages from the database.

LoadData(long chatId)
Loads previous messages from the database based on a chat ID and populates mychat.Messages.

AddInputMessage
Adds a new message to the database and updates the UI when the user clicks Send message.

AddPrompt
Sends a message to the Llama API, saves the response to the database, and updates the UI with the AI’s reply.


API Integration
LlamaChat integrates with the Llama API to send user inputs and receive generated responses.
API calls are made using HttpClient and can handle both GET and POST requests.

URL: http://localhost:11434/api/tags for GET, and http://localhost:11434/api/generate for POST.
Timeout: 5 minutes, ensuring robust handling of AI interactions.


User Documentation
Sending Messages
1. Select a user from the dropdown menu.
2. Type a message in the input field.
3. Click the "Send message" button to send the message.


Interacting with Llama in Llama.razor
1. Type a prompt in the Llama input field.
2.Click the "Send prompt" button to submit the prompt and receive a response.


Known Issues
NullReferenceException: Ensure that all objects are properly initialized before use.
HTTP Errors: Check that backend services are correctly configured and running.


License
This project is licensed under the MIT License. See the LICENSE file for more information.