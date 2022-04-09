package com.example.javamvc.controllers;

import Core.Customer;
import Core.CustomerService;
import com.example.javamvc.dtos.CustomerCreateDto;
import com.example.javamvc.dtos.CustomerGetDto;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;

//http://localhost:8080/create
//http://localhost:8080/getAll
@RestController
@SpringBootApplication
public class JavaMvcApplication {
    private final CustomerService customerService;

    public static void main(String[] args) {
        SpringApplication.run(JavaMvcApplication.class, args);
    }

    public JavaMvcApplication() {
        customerService = new CustomerService();
    }

    @PostMapping("/create")
    public int create(@RequestBody CustomerCreateDto customerCreateDto) {
        return customerService.add(new Customer(customerCreateDto.getName(), customerCreateDto.getEmail()));
    }

    @GetMapping("/getAll")
    public ArrayList<CustomerGetDto> getAll() {
        var customerDtos = new ArrayList<CustomerGetDto>();
        var customers = customerService.getAll();

        for (Customer c: customers) {
            customerDtos.add(new CustomerGetDto(c.getId(), c.getName(), c.getEmail()));
        }

        return customerDtos;
    }


}
