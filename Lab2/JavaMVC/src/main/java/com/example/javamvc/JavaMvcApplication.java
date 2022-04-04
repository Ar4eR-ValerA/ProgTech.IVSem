package com.example.javamvc;

import Core.Customer;
import Core.CustomerService;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.web.bind.annotation.*;

import java.util.ArrayList;

//http://localhost:8080/create?name=Human
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
    public int create(String name, String email) {
        return customerService.add(new Customer(name, email));
    }

    @GetMapping("/getAll")
    public ArrayList<Customer> getAll() {
        return customerService.getAll();
    }


}
