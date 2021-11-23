using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ScheduleAPIConsoleDemo
{
    public class Demos
    {
        public static void Execute(IOrganizationService service)
        {
            /*
             * Demo 1
             * Here were attempt to create a Project and a single Project Task using standard CRUD operations in a TransactionRequest
             * Although the Project will get created successfully, the Project Task will fail, as Microsoft block CRUD operations targeting 
             * this table.
            */

            // Create an ExecuteTransactionRequest object.
            ExecuteTransactionRequest transactionReq = new ExecuteTransactionRequest()
            {
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };

            //Declare and add a new CreateRequest for the Project to the ExecuteTransactionRequest
            Guid projectID = Guid.NewGuid();
            Entity project = new Entity("msdyn_project", projectID);
            project["msdyn_subject"] = "Demo 1 Project";

            CreateRequest projectCR = new CreateRequest { Target = project };
            transactionReq.Requests.Add(projectCR);

            EntityReference projectER = project.ToEntityReference();

            //Declare and add a new CreateRequest for the Project Task to the ExecuteTransactionRequest
            Guid projectTaskID = Guid.NewGuid();
            Entity projectTask = new Entity("msdyn_projecttask", projectTaskID);
            projectTask["msdyn_project"] = projectER;
            projectTask["msdyn_subject"] = "Demo 1 Project Task";
            projectTask["msdyn_effort"] = 4d;
            projectTask["msdyn_scheduledstart"] = DateTime.Today;
            projectTask["msdyn_scheduledend"] = DateTime.Today.AddDays(5);
            projectTask["msdyn_progress"] = 0.34m;
            projectTask["msdyn_start"] = DateTime.Now.AddDays(1);

            CreateRequest projectTaskCR = new CreateRequest { Target = projectTask };
            transactionReq.Requests.Add(projectTaskCR);

            Console.WriteLine("Attempting to Create Project and Project Task...");

            //Attempt to execute the Transaction
            try
            {
                service.Execute(transactionReq);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine($"Could not complete ExecuteTransactionRequest. {ex.Message}.");
            }

            Console.WriteLine("Demo 1 Complete. Press Return to continue.");
            Console.ReadLine();

            /*
             * Demo 2
             * Here, we use the Schedule API instead to Create a project, team member four tasks and two resource assignments
             * Demo is adapted from the sample found here: https://docs.microsoft.com/en-us/dynamics365/project-operations/project-management/schedule-api-preview#sample-scenario?WT.mc_id=BA-MVP-5003861
             * Note for Project Task Create, it is not possible to map values to the following columns: msdyn_progress, msdyn_start
             * And for Resource Assignment: msdyn_start, msdyn_finish
            */

            //Create the Project
            Console.WriteLine("Attempting to Create Project using msdyn_CreateProjectV1 API...");
            project["msdyn_subject"] = "Demo 2 Project";
            CallCreateProjectAction(project, service);
            Console.WriteLine($"Project with ID {project.Id} successfully created!");

            //Create the Team Member
            Console.WriteLine("Attempting to Create Team Member for Project using msdyn_CreateTeamMemberV1 API...");
            var teamMember = new Entity("msdyn_projectteam", Guid.NewGuid());
            teamMember["msdyn_name"] = $"TM {DateTime.Now.ToShortTimeString()}";
            teamMember["msdyn_project"] = projectER;
            string createTeamMemberResponse = CallCreateTeamMemberAction(teamMember, service);
            Console.WriteLine($"Team Member with ID {createTeamMemberResponse} created successfully!");

            //Create an OperationSet for all subsequence actions
            var description = $"My Create demo {DateTime.Now.ToShortTimeString()}";
            var createOperationSetId = CallCreateOperationSetAction(project.Id, description, service);
            Console.WriteLine($"Successfully generated OperationSet with ID {createOperationSetId}");

            //Build Tasks for the OperationSet
            var task1 = GetTask("1WW", projectER, service);
            var task2 = GetTask("2XX", projectER, service, task1.ToEntityReference());
            var task3 = GetTask("3YY", projectER, service);
            var task4 = GetTask("4ZZ", projectER, service );

            //Build ResourceAssignments for the OperationSet
            var assignment1 = GetResourceAssignment("R1", teamMember, task2, project);
            var assignment2 = GetResourceAssignment("R2", teamMember, task3, project);

            //Actually create the Tasks / Resource Assignments
            //Note that we must explicetely Execute the OperationSet for all rows to be created.
            var task1Response = CallPssCreateAction(task1, createOperationSetId, service);
            var task2Response = CallPssCreateAction(task2, createOperationSetId, service);
            var task3Response = CallPssCreateAction(task3, createOperationSetId, service);
            var task4Response = CallPssCreateAction(task4, createOperationSetId, service);

            var assignment1Response = CallPssCreateAction(assignment1, createOperationSetId, service);
            var assignment2Response = CallPssCreateAction(assignment2, createOperationSetId, service);

            Console.Write($"OperationSet ID {createOperationSetId} ready to execute...");
            Console.ReadLine();

            CallExecuteOperationSetAction(createOperationSetId, service);

            Console.Write($"OperationSet ID {createOperationSetId} executed successfully!");
            Console.ReadLine();

            Console.WriteLine("Demo 2 Complete. Press Return to continue.");

            /*
             * Demo 3
             * Here, we use the Schedule API instead to Update the Project created in Demo 2
             * IMPORTANT: Ensure that the previous Create OperationSet has a Status of "Complete" before executing this demo.
             * Demo is adapted from the sample found here: https://docs.microsoft.com/en-us/dynamics365/project-operations/project-management/schedule-api-preview#sample-scenario?WT.mc_id=BA-MVP-5003861
            */

            //Create an OperationSet for all subsequence actions
            description = $"My Update demo {DateTime.Now.ToShortTimeString()}";
            var updateOperationSetId = CallCreateOperationSetAction(project.Id, description, service);
            Console.WriteLine($"Successfully generated OperationSet with ID {createOperationSetId}");

            task2["msdyn_subject"] = "Updated Task";
            var task2UpdateResponse = CallPssUpdateAction(task2, updateOperationSetId, service);

            project["msdyn_subject"] = $"Proj update {DateTime.Now.ToShortTimeString()}";
            var projectUpdateResponse = CallPssUpdateAction(project, updateOperationSetId, service);

            Console.Write($"OperationSet ID {updateOperationSetId} ready to execute...");
            Console.ReadLine();

            CallExecuteOperationSetAction(updateOperationSetId, service);

            Console.Write($"OperationSet ID {updateOperationSetId} executed successfully!");
            Console.ReadLine();

            Console.WriteLine("Demo 3 Complete. Press Return to continue.");
            Console.ReadLine();

            /*
             * Demo 4
             * Finally, we tidy-up by deleting everything created in Demo 2
             * Demo is adapted from the sample found here: https://docs.microsoft.com/en-us/dynamics365/project-operations/project-management/schedule-api-preview#sample-scenario?WT.mc_id=BA-MVP-5003861
            */

            //Create an OperationSet for all subsequence actions
            description = $"My Update demo {DateTime.Now.ToShortTimeString()}";
            var deleteOperationSetId = CallCreateOperationSetAction(project.Id, description, service);
            Console.WriteLine($"Successfully generated OperationSet with ID {deleteOperationSetId}");

            var task4DeleteResponse = CallPssDeleteAction(task4.Id.ToString(), task4.LogicalName, deleteOperationSetId, service);
            var assignment2DeleteResponse = CallPssDeleteAction(assignment2.Id.ToString(), assignment2.LogicalName, deleteOperationSetId, service);
            
            Console.Write($"OperationSet ID {deleteOperationSetId} ready to execute...");
            Console.ReadLine();

            CallExecuteOperationSetAction(deleteOperationSetId, service);

            Console.Write($"OperationSet ID {deleteOperationSetId} executed successfully!");
            Console.ReadLine();

            Console.WriteLine("Demo 5 Complete. Press Return to continue.");
            Console.ReadLine();
        }

        /// <summary>
        /// Calls the action to create a new project
        /// </summary>
        /// <param name="project">Project</param>
        /// <returns>project Id</returns>
        private static void CallCreateProjectAction(Entity project, IOrganizationService service)
        {
            OrganizationRequest createProjectRequest = new OrganizationRequest("msdyn_CreateProjectV1");
            createProjectRequest["Project"] = project;
            service.Execute(createProjectRequest);
        }
        /// <summary>
        /// Calls the action to create a new project team member
        /// </summary>
        /// <param name="teamMember">Project team member</param>
        /// <returns>project team member Id</returns>
        private static string CallCreateTeamMemberAction(Entity teamMember, IOrganizationService service)
        {
            OrganizationRequest request = new OrganizationRequest("msdyn_CreateTeamMemberV1");
            request["TeamMember"] = teamMember;
            OrganizationResponse response = service.Execute(request);
            return (string)response["TeamMemberId"];
        }

        /// <summary>
        /// Calls the action to create an operationSet
        /// </summary>
        /// <param name="projectId">project id for the operations to be included in this operationSet</param>
        /// <param name="description">description of this operationSet</param>
        /// <returns>operationSet id</returns>
        private static string CallCreateOperationSetAction(Guid projectId, string description, IOrganizationService service)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_CreateOperationSetV1");
            operationSetRequest["ProjectId"] = projectId.ToString();
            operationSetRequest["Description"] = description;
            OrganizationResponse response = service.Execute(operationSetRequest);
            return response["OperationSetId"].ToString();
        }
        private static Entity GetTask(string name, EntityReference projectReference, IOrganizationService service, EntityReference parentReference = null)
        {
            var task = new Entity("msdyn_projecttask", Guid.NewGuid());
            task["msdyn_project"] = projectReference;
            task["msdyn_subject"] = name;
            task["msdyn_effort"] = 4d;
            task["msdyn_scheduledstart"] = DateTime.Today;
            task["msdyn_scheduledend"] = DateTime.Today.AddDays(5);
            task["msdyn_projectbucket"] = GetBucket(projectReference, service).ToEntityReference();
            task["msdyn_LinkStatus"] = new OptionSetValue(192350000);

            if (parentReference == null)
            {
                task["msdyn_outlinelevel"] = 1;
            }
            else
            {
                task["msdyn_parenttask"] = parentReference;
            }

            return task;
        }

        private static Entity GetBucket(EntityReference projectReference, IOrganizationService service)
        {
            var bucketCollection = GetDefaultBucket(projectReference, service);
            if (bucketCollection.Entities.Count > 0)
            {
                return bucketCollection[0].ToEntity<Entity>();
            }

            throw new Exception($"Please open project with id {projectReference.Id} in the Dynamics UI and navigate to the Tasks tab");
        }

        private static EntityCollection GetDefaultBucket(EntityReference projectReference, IOrganizationService service)
        {
            var columnsToFetch = new ColumnSet("msdyn_project", "msdyn_name");
            var getDefaultBucket = new QueryExpression("msdyn_projectbucket")
            {
                ColumnSet = columnsToFetch,
                Criteria =
        {
            Conditions =
            {
                new ConditionExpression("msdyn_project", ConditionOperator.Equal, projectReference.Id),
                new ConditionExpression("msdyn_name", ConditionOperator.Equal, "Bucket 1")
            }
        }
            };

            return service.RetrieveMultiple(getDefaultBucket);
        }

        private static Entity GetResourceAssignment(string name, Entity teamMember, Entity task, Entity project)
        {
            var assignment = new Entity("msdyn_resourceassignment", Guid.NewGuid());
            assignment["msdyn_projectteamid"] = teamMember.ToEntityReference();
            assignment["msdyn_taskid"] = task.ToEntityReference();
            assignment["msdyn_projectid"] = project.ToEntityReference();
            assignment["msdyn_name"] = name;

            return assignment;
        }

        /// <summary>
        /// Calls the action to create an entity, only Task and Resource Assignment for now
        /// </summary>
        /// <param name="entity">Task or Resource Assignment</param>
        /// <param name="createOperationSetId">operationSet id</param>
        /// <returns>OperationSetResponse</returns>
        private static OperationSetResponse CallPssCreateAction(Entity entity, string operationSetId, IOrganizationService service)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_PssCreateV1");
            operationSetRequest["Entity"] = entity;
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(service.Execute(operationSetRequest));
        }

        private static OperationSetResponse GetOperationSetResponseFromOrgResponse(OrganizationResponse orgResponse)
        {
            return JsonConvert.DeserializeObject<OperationSetResponse>((string)orgResponse.Results["OperationSetResponse"]);
        }

        /// <summary>
        /// Calls the action to execute requests in an operationSet
        /// </summary>
        /// <param name="createOperationSetId">operationSet id</param>
        /// <returns>OperationSetResponse</returns>
        private static OperationSetResponse CallExecuteOperationSetAction(string operationSetId, IOrganizationService service)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_ExecuteOperationSetV1");
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(service.Execute(operationSetRequest));
        }

        /// <summary>
        /// Calls the action to update an entity, only Task for now
        /// </summary>
        /// <param name="entity">Task or Resource Assignment</param>
        /// <param name="operationSetId">operationSet Id</param>
        /// <returns>OperationSetResponse</returns>
        private static OperationSetResponse CallPssUpdateAction(Entity entity, string operationSetId, IOrganizationService service)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_PssUpdateV1");
            operationSetRequest["Entity"] = entity;
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(service.Execute(operationSetRequest));
        }

        /// <summary>
        /// Calls the action to update an entity, only Task and Resource Assignment for now
        /// </summary>
        /// <param name="recordId">Id of the record to be deleted</param>
        /// <param name="entityLogicalName">Entity logical name of the record</param>
        /// <param name="operationSetId">OperationSet Id</param>
        /// <returns>OperationSetResponse</returns>
        private static OperationSetResponse CallPssDeleteAction(string recordId, string entityLogicalName, string operationSetId, IOrganizationService service)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_PssDeleteV1");
            operationSetRequest["RecordId"] = recordId;
            operationSetRequest["EntityLogicalName"] = entityLogicalName;
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(service.Execute(operationSetRequest));
        }
    }
}

#region OperationSetResponse DataContract --- Sample code ----
[DataContract]
public class OperationSetResponse
{
    [DataMember(Name = "createOperationSetId")]
    public Guid createOperationSetId { get; set; }

    [DataMember(Name = "operationSetDetailId")]
    public Guid OperationSetDetailId { get; set; }

    [DataMember(Name = "operationType")]
    public string OperationType { get; set; }

    [DataMember(Name = "recordId")]
    public string RecordId { get; set; }

    [DataMember(Name = "correlationId")]
    public string CorrelationId { get; set; }
}

#endregion