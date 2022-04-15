package Core;

import java.util.ArrayList;

public class CustomerService {
    private final ArrayList<Customer> customers;

    public CustomerService() {
        customers = new ArrayList<>();
    }

    public int add(Customer customer) {
        int id = customers.size();
        customer.setId(id);

        customers.add(customer);
        return id;
    }

    public Customer getById(int id) {
        for (var c: customers) {
            if (c.getId() == id) {
                return c;
            }
        }

        throw new RuntimeException("There is no such customer");
    }

    public ArrayList<Customer> getByName(String name) {
        var ans = new ArrayList<Customer>();

        for (var c: customers) {
            if (c.getName().equals(name)) {
                ans.add(c);
            }
        }

        return ans;
    }

    public ArrayList<Customer> getAll() {
        return customers;
    }

    public void changeName(int id, String name) {
        Customer customer = getById(id);
        customers.remove(customer);

        customer.setName(name);
        customers.add(customer);
    }

    public void changeEmail(int id, String email) {
        Customer customer = getById(id);
        customers.remove(customer);

        customer.setEmail(email);
        customers.add(customer);
    }

    public void delete(int id) {
        Customer customer = getById(id);
        customers.remove(customer);
    }
}
