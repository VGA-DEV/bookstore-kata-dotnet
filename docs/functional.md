# Functional Rules for the Legacy Bookstore API

## 1. Purpose of the system and who uses it

This system is a web application that manages a bookstore’s inventory through an API.
It is used by people who need to add books, change stock and status, and look up books in the catalog.

## 2. Glossary

- **Book**: one item in the bookstore catalog.
- **Category**: a fixed label used to group books into a business area such as Fiction or Science.
- **Active book**: a book currently available in the catalog as an active item.
- **Inactive book**: a book marked as not active. The system may also store the date when it was made inactive.
- **Quantity (stock)**: how many copies the system says are available for that book.

## 3. The book: attributes, meaning, requiredness, defaults, and changeability

The system stores these book attributes today:

- **id**
  - Meaning: the unique identity of the book record.
  - Required on creation: no, the system creates one automatically.
  - Default value: generated automatically when a new book is created.
  - Can change after creation: no.

- **title**
  - Meaning: the book name shown to users.
  - Required on creation: yes, the create operation only proceeds when a title is provided.
  - Default value: none.
  - Can change after creation: no, not through the current update operation.

- **author**
  - Meaning: the writer or creator of the book.
  - Required on creation: no.
  - Default value: none.
  - Can change after creation: no, not through the current update operation.

- **categoryId**
  - Meaning: the category assigned to the book.
  - Required on creation: no, but if it is provided it must match one of the fixed categories.
  - Default value: none in business terms; the stored value becomes 0 when no category is sent.
  - Can change after creation: no, not through the current update operation.

- **quantity**
  - Meaning: the number of copies in stock.
  - Required on creation: not enforced.
  - Default value: 0.
  - Can change after creation: yes, it is the main value changed by the update operation.

- **description**
  - Meaning: free text describing the book.
  - Required on creation: no.
  - Default value: none.
  - Can change after creation: no, not through the current update operation.

- **language**
  - Meaning: the language of the book.
  - Required on creation: no.
  - Default value: none.
  - Can change after creation: no, not through the current update operation.

- **active**
  - Meaning: whether the book is considered active.
  - Required on creation: no.
  - Default value: true.
  - Can change after creation: yes, the update operation can change it.

- **inactiveDate**
  - Meaning: the date the book was made inactive.
  - Required on creation: no.
  - Default value: none.
  - Can change after creation: yes, but only indirectly through the update operation or by sending it during creation.

- **publisher**
  - Meaning: the publisher name.
  - Required on creation: no.
  - Default value: none.
  - Can change after creation: no, not through the current update operation.

- **publisherDate**
  - Meaning: the publisher date.
  - Required on creation: no.
  - Default value: none.
  - Can change after creation: no, not through the current update operation.

## 4. Categories

The system uses this fixed list of categories:

1. Fiction
2. Non-Fiction
3. Science
4. Biography
5. Children
6. Programming

Business rules:

- Category names are matched by exact spelling and exact letter case.
- Users cannot create, edit, or delete categories through the API.
- The same category list exists in application code and in a database collection created by the database seed script.
- The API does not read categories from the database collection; it uses the list in code.

## 5. Operations

### 5.1 Create a book

Business rule:

- A user can create a book only when a title is provided.
- The system assigns a new identity automatically.
- If no quantity is provided, the stock is saved as zero.
- If no active flag is provided, the book is saved as active.
- If no category is provided, the book is saved without a category value.
- Other details such as author, description, language, inactive date, publisher, and publisher date may be left empty.

Given / When / Then examples:

- Given a user sends a title and other optional details, when the create operation is used, then the system saves the book and returns the created record.
- Given a user sends no title, when the create operation is used, then the system does not create a book and returns a not found response.
- Given a user sends a known category name, when the create operation is used, then the book is linked to that category.
- Given a user sends an unknown category name, when the create operation is used, then the request fails with a server error. This is because the system cannot resolve that category.

What the user sees when something goes wrong:

- Missing title: no book is created; the system returns a not found response.
- Unknown category: the request fails with a server error.

### 5.2 Update a book

Business rule:

- A user can update a book only when an identity is provided.
- The update operation changes stock and active status.
- If the active flag is not sent, the system treats it as false and marks the book inactive.
- When a book is made inactive, the system stamps the inactive date automatically.
- Other book details are not changed by the current update operation.

Given / When / Then examples:

- Given a user sends a book identity, a new quantity, and active = true, when the update operation is used, then the stock is updated and the book stays active.
- Given a user sends a book identity and a new quantity but does not send active, when the update operation is used, then the book is silently made inactive and the inactive date is set.
- Given a user sends no identity, when the update operation is used, then the system returns a not found response.
- Given a user sends an identity that does not exist, when the update operation is used, then the system still returns success and the record is treated as updated even though no existing book was found.

What the user sees when something goes wrong:

- Missing identity: not found response.
- Non-existing identity: success response with a created-style result, even when no existing record was matched.

### 5.3 Get one book

Business rule:

- The user can ask for one book by identity or by title.
- Title matching is exact and case-sensitive.
- Search by author is announced in the API but is not implemented.
- Search by identity is intended to work, but the current system does not actually find books by identity.

Given / When / Then examples:

- Given a user searches by exact title, when the title exists, then the matching book is returned.
- Given a user searches by title with different letter case, when the title does not exactly match, then no book is returned.
- Given a user searches by author, when the author parameter is provided, then the system throws an error because that feature is not implemented.
- Given a user searches by identity, when the identity exists, then no book is returned today because the current matching logic never succeeds.

What the user sees when something goes wrong:

- Search by author: a server error.
- Search by identity: no result, even when the book exists.

### 5.4 List books

Business rule:

- The user can list all books.
- The user can also request books for one category.
- Category matching is exact and case-sensitive.
- If the category name is unknown, the system returns all books instead of returning none.

Given / When / Then examples:

- Given a user requests the list without a category, when the list operation is used, then all books are returned.
- Given a user requests the list for Fiction, when the category exists, then only Fiction books are returned.
- Given a user requests the list for fiction in lowercase, when the category does not exactly match, then all books are returned.
- Given a user requests the list for an unknown category, when the category does not exist, then all books are returned.

What the user sees when something goes wrong:

- Unknown category or incorrect category spelling: the response still contains the full list of books.

## 6. Known issues and risks

These are the known gaps in the current system and their business impact:

- **Updating a book without sending active silently deactivates it and stamps the inactive date.**
  - Business impact: a user can accidentally make a book unavailable while only trying to change stock.

- **Searching a book by id never finds it.**
  - Business impact: users cannot reliably retrieve a book by its identity, which makes support, lookup, and follow-up actions difficult.

- **Filtering a list by an unknown category returns all books instead of none.**
  - Business impact: users may believe there are books in a category that does not exist, which can distort reporting and manual checks.

- **Creating a book with an unknown category fails with a server error.**
  - Business impact: users receive a technical failure instead of a clear business validation message.

- **Updating a book that does not exist reports success.**
  - Business impact: users may believe a change was saved when no matching book was actually found.

- **Stock (quantity) can be negative.**
  - Business impact: the system can show impossible inventory values, which can break trust in stock reporting.

- **A book can be created active with an inactive date.**
  - Business impact: the same record can carry conflicting status information, which makes the catalog harder to interpret.

- **Filter by author is announced but not implemented; delete is mentioned in the README but does not exist.**
  - Business impact: users may expect features that are not available, which can cause confusion and delivery risk.

## 7. Open questions for the Product Owner

For each item below, please confirm whether it is intended behavior or a bug to fix in the migration:

1. Should updating a book without sending active keep the current status instead of deactivating it?
2. Should searching by book id work as a normal lookup?
3. Should an unknown category return no books instead of all books?
4. Should creating a book with an unknown category be a business validation error instead of a server error?
5. Should updating a book that does not exist report not found instead of success?
6. Should stock be prevented from going below zero?
7. Should active books be allowed to have an inactive date?
8. Should author filtering be implemented in the migration?
9. Should delete exist, as suggested in the README?

## 8. To be confirmed

The following points are visible in the code but still need product confirmation before they are relied on as business rules:

- Whether missing author, description, language, publisher, and publisher date should remain optional in the new system.
- Whether returning a not found response for a missing title on create is intentional business behavior.
- Whether the current exact, case-sensitive matching for titles and categories is the desired user experience.
