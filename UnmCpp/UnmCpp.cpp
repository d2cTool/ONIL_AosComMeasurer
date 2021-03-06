// UnmCpp.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <cstdlib>
#include <iostream>
#include <ctime>
#include <Windows.h>
#include <chrono>
#include <thread>
#include <sstream>
#include <string.h>
#include <limits>

using namespace std::chrono_literals;

typedef bool(*functionPointer1)(const char* str1, const char* str2);
typedef bool(*functionPointer2)();
typedef bool(*functionPointer3)(int i1, int i2);

std::string generateValue();

extern "C" {
	void InitMeasurer(const char*);
	void Init();
	byte ShowMeasurer(byte);
	int SetMeasure(double, double, double, double, double);
	byte SetLamp(int, byte);
	void PlayMSound(int);


	double r_var1;
	double r_var2;
	double r_var3;
	double r_var4;
	double r_var5;

	bool __stdcall OnConnectCallbackFunction(functionPointer1);
	bool __stdcall OnDisconnectCallbackFunction(functionPointer2);
	bool __stdcall OnStateChangedCallbackFunction(functionPointer3);
}

bool GetDataCallback(const char* str1, const char* str2) {
	std::cout << "Connect: measurer serial: " << str1 << ", board serial: " << str2 << std::endl;

	std::string pathToBase = "D:\\Dropbox\\Onil\\Projects\\Measurer Driver\\AosComMeasurer\\MDeviceTest\\base";
	InitMeasurer(pathToBase.c_str());

	r_var1 = std::rand() * 0.0001;
	r_var2 = std::rand() * 0.0001;
	r_var3 = std::rand() * 0.01;
	r_var4 = std::rand() * 0.01;
	r_var5 = std::rand() * 0.1;

	return true;
}

bool LostConnectionCallback() {
	std::cout << "Disconnect." << std::endl;
	//Init();
	return true;
}

bool StateChangedCallback(int i1, int i2) {
	std::cout << "New state: plus: " << i1 << ", minus:" << i2 << std::endl;

	double inf = std::numeric_limits<double>::infinity();
	double nan = std::numeric_limits<double>::quiet_NaN();

	SetMeasure(r_var1, r_var2, r_var3, r_var4, r_var5);

	return true;
}

int main()
{
	std::srand((int)std::time(0));

	OnConnectCallbackFunction(&GetDataCallback);
	OnDisconnectCallbackFunction(&LostConnectionCallback);
	OnStateChangedCallbackFunction(&StateChangedCallback);

	Init();

	std::string input;
	int x;

	while (true)
	{
		getline(std::cin, input);
		x = std::stoi(input);

		switch (x)
		{
		case 1:
			Init();
			break;
		case 2:
			//Stop();
			break;
		case 3:
			SetMeasure(0.1, 10, 10, 100, 100);
			break;
		default:
			break;
		}
	}

	return EXIT_SUCCESS;
}

std::string generateValue() {
	int random_variable = std::rand() & 0xFF;
	std::stringstream strs;
	std::string command = "v";
	strs << command << random_variable;
	std::string temp_str = strs.str();
	return temp_str;
}


