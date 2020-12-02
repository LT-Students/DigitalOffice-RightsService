using System;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetUserResponse
    {
        Guid Id { get; }
        string FirstName { get; }
        string MiddleName { get; }
        string LastName { get; }
        bool IsActive { get; }

        static object CreateObj(Guid id, string firstName, string middleName, string lastName, bool isActive)
        {
            return new
            {
                Id = id,
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                IsActive = isActive
            };
        }
    }
}
