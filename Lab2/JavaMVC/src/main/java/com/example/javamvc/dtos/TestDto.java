package com.example.javamvc.dtos;

import java.util.ArrayList;

public class TestDto {
    private ArrayList<String> stringArray;

    public TestDto(ArrayList<String> stringArray) {
        this.stringArray = stringArray;
    }

    public ArrayList<String> getStringArray() {
        return stringArray;
    }

    public void setStringArray(ArrayList<String> stringArray) {
        this.stringArray = stringArray;
    }
}
