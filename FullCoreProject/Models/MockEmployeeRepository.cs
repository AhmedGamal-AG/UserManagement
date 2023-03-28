using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FullCoreProject.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()
            {
                new Employee(){Id=1,Name="Mary",Department=Dept.HR,Email="Mary@gmail.com"},
                new Employee(){Id=2,Name="John",Department=Dept.IT,Email="John@gmail.com"},
                new Employee(){Id=3,Name="Sam",Department=Dept.IT,Email="Sam@gmail.com"}
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id=_employeeList.Max(emp => emp.Id)+1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            var employee = _employeeList.FirstOrDefault(emp => emp.Id == id);
            if(employee!=null)
            {
                _employeeList.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int id)
        {
            return _employeeList.FirstOrDefault(emp => emp.Id == id);
        }

        public Employee Update(Employee UpdatedEmployee)
        {
            var employee = _employeeList.FirstOrDefault(emp => emp.Id == UpdatedEmployee.Id);
            if (employee != null)
            {
                employee.Name = UpdatedEmployee.Name;
                employee.Email = UpdatedEmployee.Email;
                employee.Department = UpdatedEmployee.Department;
            }
            return employee;
        }
    }
}
