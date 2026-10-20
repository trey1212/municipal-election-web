using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace municipal_election_web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedPositionsAndCandidates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "PositionId", "MunicipalityId", "Name", "SeatsAvailable" },
                values: new object[,]
                {
                    { 1, 1, "Mayor", 1 },
                    { 2, 1, "Councilor", 8 },
                    { 3, 1, "Trustee", 6 },
                    { 4, 2, "Mayor", 1 },
                    { 5, 2, "Councilor", 8 },
                    { 6, 2, "Trustee", 5 }
                });

            migrationBuilder.InsertData(
                table: "Candidates",
                columns: new[] { "CandidateId", "Biography", "Email", "MunicipalityId", "Name", "PhotoUrl", "PositionId", "Website" },
                values: new object[,]
                {
                    { 1, "Former city planner focused on sustainable transit expansion, community safety, and local business growth.", "campaign@marcusvance.ca", 1, "Marcus Vance", "/images/candidates/marcus-vance.jpg", 1, "https://marcusvanceforsurrey.ca" },
                    { 2, "Small business owner and civic activist championing affordable housing initiatives and municipal tax relief.", "info@gillforsurrey.com", 1, "Amrit Gill", "/images/candidates/amrit-gill.jpg", 1, "https://gillforsurrey.com" },
                    { 3, "Advocating for green urban spaces, improved cycling networks, and revitalization of community recreational centres.", "linda@nguyenforward.ca", 1, "Linda Nguyen", "/images/candidates/linda-nguyen.jpg", 2, "https://lindanguyen.ca" },
                    { 4, "Focusing on transparent city budgeting, neighborhood watch initiatives, and infrastructure upgrades.", "david@chensurrey.org", 1, "David Chen", "/images/candidates/david-chen.jpg", 2, "https://davidchensurrey.org" },
                    { 5, "Parent and STEM educator passionate about reducing classroom sizes and increasing funding for special education.", "contact@priyasharma.ca", 1, "Priya Sharma", "/images/candidates/priya-sharma.jpg", 3, "https://sharmafortrustee.ca" },
                    { 6, "Retired high school principal dedicated to expanding modern library resources and mental health supports for students.", "robert@oconnortrustee.ca", 1, "Robert O'Connor", "/images/candidates/robert-oconnor.jpg", 3, "https://oconnorforstudents.com" },
                    { 7, "Dedicated to sensible downtown redevelopment, walkable corridors, and strengthening emergency response services.", "thomas@wrightforlangley.ca", 2, "Thomas Wright", "/images/candidates/thomas-wright.jpg", 4, "https://wrightforlangley.ca" },
                    { 8, "Long-time community volunteer prioritizing local heritage preservation and sensible fiscal management.", "hello@sarahjenkins.ca", 2, "Sarah Jenkins", "/images/candidates/sarah-jenkins.jpg", 4, "https://sarahjenkins.ca" },
                    { 9, "Civil engineer working toward safer roadways, traffic calming in residential areas, and better flood management.", "michael@changforlangley.org", 2, "Michael Chang", "/images/candidates/michael-chang.jpg", 5, "https://changforlangley.org" },
                    { 10, "Advocate for park improvements, community gardens, and fostering supportive environments for local small businesses.", "claire@tremblaycouncil.com", 2, "Claire Tremblay", "/images/candidates/claire-tremblay.jpg", 5, "https://clairetremblay.com" },
                    { 11, "Youth mentor and volunteer focusing on upgrading school technology, digital literacy, and classroom safety.", "harpreet@dhillontrustee.ca", 2, "Harpreet Dhillon", "/images/candidates/harpreet-dhillon.jpg", 6, "https://dhillonfortrustee.ca" },
                    { 12, "Former PAC chair working to secure provincial grants for arts programs and after-school childcare options.", "emily@watsonfortrustee.ca", 2, "Emily Watson", "/images/candidates/emily-watson.jpg", 6, "https://watsonforlangleykids.com" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Candidates",
                keyColumn: "CandidateId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "PositionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "PositionId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "PositionId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "PositionId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "PositionId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "PositionId",
                keyValue: 6);
        }
    }
}
