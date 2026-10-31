import { useState } from "react";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { useCustomerSearch } from "@/features/customers/hooks/use-customers";

export function CreateOrderButton() {
  const [open, setOpen] = useState(false);
  const [customerSearch, setCustomerSearch] = useState("");
  const [selectedCustomer, setSelectedCustomer] = useState("");
  const searchTerm = customerSearch.trim();

  const {
    data: customers,
    error,
    isFetching,
  } = useCustomerSearch(searchTerm, open && searchTerm.length > 0);

  return (
    <>
      <Button className="space-x-1" onClick={() => setOpen(true)}>
        <span>Create order</span> <Plus size={18} />
      </Button>
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent className="sm:max-w-lg">
          <DialogHeader className="text-start">
            <DialogTitle>Create new order</DialogTitle>
            <DialogDescription>
              This form is a preview and does not save orders yet.
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div className="space-y-2">
              <label htmlFor="order-customer">Customer</label>
              <Input
                id="order-customer"
                value={customerSearch}
                placeholder="Search customers by name"
                onChange={(event) => {
                  setCustomerSearch(event.target.value);
                  setSelectedCustomer("");
                }}
              />
              {searchTerm && (
                <div
                  className="max-h-40 overflow-y-auto rounded-md border"
                  role="listbox"
                  aria-label="Matching customers"
                >
                  {isFetching ? (
                    <p className="p-3 text-sm text-muted-foreground">
                      Searching customers...
                    </p>
                  ) : error instanceof Error ? (
                    <p className="p-3 text-sm text-destructive" role="alert">
                      {error.message}
                    </p>
                  ) : customers?.data.length ? (
                    customers.data.map((customer) => (
                      <button
                        key={customer.id}
                        type="button"
                        role="option"
                        aria-selected={selectedCustomer === customer.name}
                        className="block w-full px-3 py-2 text-left text-sm hover:bg-muted"
                        onClick={() => {
                          setSelectedCustomer(customer.name ?? "");
                          setCustomerSearch(customer.name ?? "");
                        }}
                      >
                        {customer.name}
                      </button>
                    ))
                  ) : (
                    <p className="p-3 text-sm text-muted-foreground">
                      No matching customers.
                    </p>
                  )}
                </div>
              )}
            </div>
            <div className="space-y-2">
              <label htmlFor="order-article">Article</label>
              <Input id="order-article" placeholder="Article name or ID" />
            </div>
            <div className="space-y-2">
              <label htmlFor="order-quantity">Quantity</label>
              <Input
                id="order-quantity"
                type="number"
                min="1"
                placeholder="1"
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)}>
              Close
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}
